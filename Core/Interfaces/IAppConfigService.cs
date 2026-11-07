using Core.Models;

namespace Core.Interfaces;

/// <summary>
/// Actions for reading/generating Configuration file for this app.
/// </summary>
public interface IAppConfigService
{
    /// <summary>
    /// Tests, Reads, and sets the passed config to the class' internal state.
    /// </summary>
    /// <param name="filePath">Relative/Absolute file path of the config file.</param>
    void ReadConfig(string filePath);

    /// <summary>
    /// Generates a blank configuration file
    /// </summary>
    /// <param name="filePath">Relative/Absolute file path to generate a configuration file.</param>
    void GenerateConfig(string filePath);

    /// <summary>
    /// Returns the current internal AppConfig state.
    /// </summary>
    /// <returns>AppConfig object of the latest config read.</returns>
    AppConfig GetConfig();
}