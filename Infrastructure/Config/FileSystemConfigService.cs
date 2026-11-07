using System.Text.Json.Nodes;
using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Config;

/// <inheritdoc />
public class FileSystemConfigService(IJsonIoActions<AppConfig> jsonIoActions) : IAppConfigService
{
    private AppConfig _config = new(
        0, "", 7727, 180
    );


    /// <inheritdoc />
    public void ReadConfig(string filePath)
    {
        _config = jsonIoActions.ReadFile(filePath);
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

        return jsonIoActions.GenerateFile(filePath, data);
    }

    /// <inheritdoc />
    public AppConfig GetConfig()
    {
        return _config;
    }
}