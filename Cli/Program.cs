using Cli.CommandRegistrar;
using Cli.Commands.Generate;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;

// DI
var services = new ServiceCollection();


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
        generate.AddCommand<GenerateConfig>("config");
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