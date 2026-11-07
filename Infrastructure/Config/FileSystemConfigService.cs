using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Config;

/// <inheritdoc />
public class FileSystemConfigService : IAppConfigService
{
    private AppConfig _config = new(
        0, "", 7727, 180
    );

    /// <inheritdoc />
    public void ReadConfig(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) throw new Exception("Config: File does not exist");

            var parsedConfig = JsonSerializer.Deserialize<AppConfig>(filePath, JsonSerializerOptions.Strict);

            _config = parsedConfig ?? throw new Exception("Config: Invalid Configuration.");
        }
        catch (JsonException _)
        {
            throw new Exception("Config: Invalid Configuration.");
        }
    }

    /// <inheritdoc />
    public bool GenerateConfig(string filePath)
    {
        var data = new JsonObject
        {
            ["client_id"] = 0,
            ["client_secret"] = "",
            ["auth_port"] = 7727,
            ["auth_timeout"] = 180
        };

        try
        {
            using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
#pragma warning disable CA1869
            var options = new JsonSerializerOptions
#pragma warning restore CA1869
            {
                WriteIndented = true
            };

            JsonSerializer.Serialize(fs, data, options);
            return true;
        }
        catch (IOException ex)
        {
            return false;
        }
    }

    /// <inheritdoc />
    public AppConfig GetConfig()
    {
        return _config;
    }
}