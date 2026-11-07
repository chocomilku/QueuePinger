using Core.Models;

namespace Core.Interfaces;

public interface IAppConfigService
{
    AppConfig Read(string filePath);
    void Generate(string filePath);
}