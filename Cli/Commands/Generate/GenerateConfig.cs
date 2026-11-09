using System.ComponentModel;
using Core.Interfaces;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Commands.Generate;

internal class GenerateConfig : Command<GenerateConfig.Settings>
{
    private readonly IAppConfigService _fileConfigService;

    public GenerateConfig(IAppConfigService fileConfigService)
    {
        _fileConfigService = fileConfigService;
    }

    public class Settings : CommandSettings
    {
        [Description("The folder where to generate the config file.")]
        [CommandArgument(0, "[dir]")]
        public string Directory { get; init; } = ".";
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        const string fileName = "config.json";
        var resolvedPath = Path.GetFullPath(settings.Directory);
        var resolvedFile = Path.Combine(resolvedPath, fileName);

        if (File.Exists(resolvedFile)) throw new Exception("File exists.");

        var success = _fileConfigService.GenerateConfig(resolvedFile);

        if (!success)
        {
            AnsiConsole.MarkupLine($"[red]Error generating {fileName}[/]");
            return 1;
        }

        AnsiConsole.MarkupLine($"[green]Generated {fileName} successfully![/]");
        return 0;
    }
}