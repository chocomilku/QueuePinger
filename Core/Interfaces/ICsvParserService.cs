using Core.Models;

namespace Core.Interfaces;

public interface ICsvParserService<T>
{
    bool Generate(string filePath);
    (bool, IReadOnlyList<string>) ValidateHeaders(string filePath);
    CsvParseResult<T> Parse(string filePath);
}