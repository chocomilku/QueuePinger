using Cli.CommandRegistrar;
using Cli.Commands.Generate;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Api;
using Infrastructure.Auth;
using Infrastructure.Config;
using Infrastructure.Csv;
using Infrastructure.Processing;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

// DI
var services = new ServiceCollection();

// store
services.AddSingleton<ITokenStore, TokenStore>();

// config
services.AddSingleton<IJsonIoActions<AppConfig>>(_ => new JsonIoActions<AppConfig>("AppConfig"));
services.AddSingleton<IJsonIoActions<MessageTemplate>>(_ =>
    new JsonIoActions<MessageTemplate>("MessageTemplate"));
services.AddSingleton<IAppConfigService, FileSystemConfigService>();
services.AddSingleton<ITemplateService, MessageTemplateService>();

// csv
services.AddSingleton<ICsvParserService<SubmissionDetail>, SubmissionCsvParserService>();

// oauth
services.AddHttpClient("OsuOAuthClient", client =>
{
    client.BaseAddress = new Uri("https://osu.ppy.sh");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
services.AddSingleton<IOAuthService, MinimalApiOAuthService>();

// api
services.AddTransient<AuthHeaderHandler>();
services.AddHttpClient("OsuClient", client =>
{
    client.BaseAddress = new Uri("https://osu.ppy.sh");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
}).AddHttpMessageHandler<AuthHeaderHandler>();
services.AddSingleton<IOsuClient, OsuClient>();

// process
services.AddSingleton<IMapsetResolver, MapsetResolver>();
services.AddSingleton<IMessagingService, OsuMessagingService>();

var registrar = new TypeRegistrar(services);

// CLI
var app = new CommandApp(registrar);
app.Configure(config =>
{
    config.SetApplicationName("QueuePinger");
    config.SetApplicationVersion("1.0.0");

    // commands
    config.AddBranch("generate", generate =>
    {
        generate.SetDescription("Generate configuration files");
        generate.AddExample("config");
        generate.AddExample("template", ".");
        generate.AddExample("csv", "./input");

        generate.AddCommand<GenerateConfig>("config");
        generate.AddCommand<GenerateTemplate>("template");
        generate.AddCommand<GenerateCsv>("csv");
    });

    // exception
    config.SetExceptionHandler((ex, resolver) =>
    {
        AnsiConsole.WriteException(ex, ExceptionFormats.NoStackTrace);

        return ex switch
        {
            InvalidOperationException => 2,
            IOException => 3,
            _ => 1
        };
    });
});


return app.Run(args);