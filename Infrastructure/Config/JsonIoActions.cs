using System.Text.Json;
using System.Text.Json.Nodes;
using Core.Interfaces;

namespace Infrastructure.Config;

public class JsonIoActions<T>(string? identifier) : IJsonIoActions<T>
{
    private readonly string _identifier = identifier ?? "Default";

    public T ReadFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath)) throw new Exception($"{_identifier}: File does not exist");

            using var stream = File.OpenRead(filePath);

            var parsedFile = JsonSerializer.Deserialize<T>(stream, JsonSerializerOptions.Web);

            return parsedFile ?? throw new Exception($"{_identifier}: Invalid File.");
        }
        catch (JsonException e)
        {
            Console.WriteLine(e.Message);
            throw new Exception("Config: Invalid Configuration.");
        }
    }

    public bool GenerateFile(string filePath, JsonObject data)
    {
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
        catch (IOException)
        {
            return false;
        }
    }
}