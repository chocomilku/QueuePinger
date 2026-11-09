using System.Text.Json.Nodes;

namespace Core.Interfaces;

public interface IJsonIoActions<out T>
{
    /// <summary>
    ///     Parses a file to a specified record.
    /// </summary>
    /// <param name="filePath">Relative/Absolute file path of the file.</param>
    T ReadFile(string filePath);

    /// <summary>
    ///     Generates a blank file
    /// </summary>
    /// <param name="filePath">
    ///     Relative/Absolute file path to generate a file. This path MUST include the file name and
    ///     extension
    /// </param>
    /// <param name="data">JSON object with data to write</param>
    /// <example>
    ///     <c>config.json</c>
    /// </example>
    /// <returns>Boolean that returns if the file is successfully generated</returns>
    bool GenerateFile(string filePath, JsonObject data);
}