using System.ComponentModel;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Utils;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands;

internal class Run : AsyncCommand<Run.Settings>
{
    private readonly IOAuthService _authService;
    private readonly IAppConfigService _fileConfigService;
    private readonly IMapsetResolver _mapsetResolver;
    private readonly IMessagingService _messagingService;
    private readonly ICsvParserService<SubmissionDetail> _parserService;
    private readonly ITemplateService _templateService;
    private readonly ITokenStore _tokenStore;

    public Run(IAppConfigService fileConfigService, ITemplateService templateService,
        ICsvParserService<SubmissionDetail> parserService, IOAuthService authService, ITokenStore tokenStore,
        IMapsetResolver mapsetResolver, IMessagingService messagingService)
    {
        _fileConfigService = fileConfigService;
        _templateService = templateService;
        _parserService = parserService;
        _authService = authService;
        _tokenStore = tokenStore;
        _mapsetResolver = mapsetResolver;
        _messagingService = messagingService;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("-c|--config [FILE]")]
        [Description("Config file to use.")]
        [DefaultValue("./config.json")]
        public required FlagValue<string> Config { get; init; }

        [CommandOption("-t|--template [FILE]")]
        [Description("Template file to use.")]
        [DefaultValue("./template.json")]
        public required FlagValue<string> Template { get; init; }

        [CommandArgument(0, "[FILE]")]
        [Description("Csv file to run.")]
        public required string Csv { get; init; } = "./list.csv";
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        #region Validate Config

        var configValue = settings.Config.IsSet ? settings.Config.Value : "./config.json";
        _fileConfigService.ReadConfig(Path.GetFullPath(configValue));
        var config = _fileConfigService.GetConfig();
        if (config is null)
        {
            AnsiConsole.MarkupLine("[red]Error reading config file.[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Successfully read config file: {Markup.Escape(configValue)}[/]");

        #endregion

        #region Read Template

        var templateValue = settings.Template.IsSet ? settings.Config.Value : "./template.json";
        var template = _templateService.ReadTemplate(Path.GetFullPath(templateValue));
        AnsiConsole.MarkupLine(
            $"[green]Successfully read template file: {Markup.Escape(template.Name)} ({Markup.Escape(templateValue)})[/]");

        #endregion

        #region Validate Csv

        var resolvedFile = Path.GetFullPath(settings.Csv);
        var (validateCsv, missingHeaders) = _parserService.ValidateHeaders(resolvedFile);
        if (!validateCsv)
        {
            AnsiConsole.MarkupLine("[red]Some headers are missing.[/]");
            foreach (var missingHeader in missingHeaders)
                AnsiConsole.MarkupLine($"    [green]•[/] {Markup.Escape(missingHeader)}");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Successfully validated csv file: {settings.Csv}[/]");

        #endregion

        #region Parse Csv

        var (submissions, csvParseErrors) = AnsiConsole.Status().Spinner(Spinner.Known.Toggle9).Start(
            "Parsing Csv file...", ctx =>
            {
                var csvParse = _parserService.Parse(resolvedFile);

                AnsiConsole.MarkupLine("[green]Finished Parsing Csv File.[/]");
                return csvParse;
            });

        if (csvParseErrors.Count != 0)
        {
            AnsiConsole.MarkupLine("[red]Failed rows:[/]");
            foreach (var failedRecord in csvParseErrors)
                AnsiConsole.MarkupLine(
                    $"    [red]• {failedRecord.RowIndex}. {Markup.Escape(failedRecord.RawContent)}[/]");

            AnsiConsole.MarkupLine("These records will be excluded.");
        }

        #endregion

        #region Auth

        var oAuthGrant = _authService.GenerateOAuthGrant();
        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine($"OAuth Grant Link: {Markup.Escape(oAuthGrant)}");
        BrowserUtils.OpenBrowser(oAuthGrant);


        await AnsiConsole.Status().Spinner(Spinner.Known.Toggle9).StartAsync(
            "Waiting for osu! User Authorization...",
            async ctx =>
            {
                var code = await _authService.AuthenticateAsync();
                ctx.Status("Received OAuth code.");

                ctx.Status("Exchanging to token...");
                var token = await _authService.ExchangeToToken(code);

                AnsiConsole.MarkupLine("[green]Successfully Authenticated[/]");
                _tokenStore.BearerToken = token.AccessToken;

                return token;
            });

        #endregion

        #region Resolve Submissions

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[yellow]Resolving Submissions[/]");
        List<MapsetSubmission> successfulSubmissions = [];
        List<SubmissionDetail> failedSubmissions = [];

        await AnsiConsole.Status().Spinner(Spinner.Known.Toggle9).AutoRefresh(false).StartAsync("Resolving...",
            async ctx =>
            {
                for (var i = 0; i < submissions.Count; i++)
                {
                    ctx.Status($"Resolving {i + 1} of {submissions.Count}...");
                    ctx.Refresh();

                    var resolvedMapset = await _mapsetResolver.ResolveMapset(submissions[i]);
                    if (resolvedMapset is null)
                        failedSubmissions.Add(submissions[i]);
                    else
                        successfulSubmissions.Add(resolvedMapset);

                    await Task.Delay(250, cancellationToken);
                }
            });

        AnsiConsole.MarkupLine("[green]Resolving Complete![/]");
        if (failedSubmissions.Count != 0)
        {
            AnsiConsole.MarkupLine("[red]Failed submissions:[/]");
            foreach (var failedSubmission in failedSubmissions)
                AnsiConsole.MarkupLine(
                    $"    [red]• {Markup.Escape(failedSubmission.MapsetLink)}[/]");

            AnsiConsole.MarkupLine("These records will be excluded.");
        }

        #endregion

        #region Confirmation

        AnsiConsole.MarkupLine("");
        AnsiConsole.MarkupLine("[green]Submissions:[/]");
        foreach (var submission in successfulSubmissions)
            AnsiConsole.MarkupLine(
                $"    [green]• {Markup.Escape(submission.Artist)} - {Markup.Escape(submission.Title)} ({Markup.Escape(submission.Creator)})[/]");

        if (!await AnsiConsole.ConfirmAsync("Is everything correct?", true, cancellationToken)) return 0;

        #endregion

        #region Send Messages

        await AnsiConsole.Progress().StartAsync(async ctx =>
        {
            var task = ctx.AddTask("Sending", maxValue: successfulSubmissions.Count);

            for (var i = 0; i < successfulSubmissions.Count; i++)
            {
                var submission = successfulSubmissions[i];
                task.Description =
                    $"({i + 1}/{successfulSubmissions.Count}): {Markup.Escape(submission.Artist)} - {Markup.Escape(submission.Title)} ({Markup.Escape(submission.Creator)}) ";
                task.Increment(1);

                var resolvedSubmission = _messagingService.ResolveTemplateToMessage(template, submission);

                await _messagingService.SendMessageTemplate(resolvedSubmission, submission.UserId);
                await Task.Delay(500, cancellationToken);
            }
        });

        #endregion

        AnsiConsole.MarkupLine("[green]Successfully Sent Messages![/]");

        return 0;
    }
}