namespace Core.Models;

public record CsvParseResult<T>(
    IReadOnlyList<T> SuccessfulRecords,
    IReadOnlyList<CsvParseError> FailedRecords);

public record CsvParseError(
    int RowIndex,
    string RawContent,
    string Message
);