using Cli.CommandRegistrar;
using Microsoft.Extensions.DependencyInjection;
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

    
});


return app.Run(args);