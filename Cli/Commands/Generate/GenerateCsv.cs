using System.ComponentModel;
using Core.Interfaces;
using Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands.Generate;

internal class GenerateCsv : Command<GenerateCsv.Settings>
{
    private ICsvParserService<SubmissionDetail> _parserService;

    public GenerateCsv(ICsvParserService<SubmissionDetail> parserService)
    {
        _parserService = parserService;
    }

    public class Settings : CommandSettings
    {
        [Description("The folder where to generate the CSV file.")]
        [CommandArgument(0, "[dir]")]
        public string Directory { get; init; } = ".";
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        const string fileName = "list.csv";
        var resolvedPath = Path.GetFullPath(settings.Directory);
        var resolvedFile = Path.Combine(resolvedPath, fileName);

        if (File.Exists(resolvedFile)) throw new Exception("File exists.");

        var success = _parserService.Generate(resolvedFile);

        if (!success)
        {
            AnsiConsole.MarkupLine($"[red]Error generating {fileName}[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Generated {fileName} successfully![/]");
        return 0;
    }
}