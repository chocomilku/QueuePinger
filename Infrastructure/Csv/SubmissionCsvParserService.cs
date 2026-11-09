using Core.Interfaces;
using Core.Models;
using CsvHelper;

namespace Infrastructure.Csv;

public class SubmissionCsvParserService : ICsvParserService<SubmissionDetail>
{
    public bool Generate(string filePath)
    {
        try
        {
            using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);
            using var stream = new StreamWriter(fs);
            using var csv = CsvWriter.Create(stream);
            csv.WriteHeader(["Link", "Status", "Remarks"]);

            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public (bool, IReadOnlyList<string>) ValidateHeaders(string filePath)
    {
        try
        {
            using var reader = CsvReader.FromFile(filePath);

            var checkHeaders = reader.TryValidateHeader<SubmissionDetail>(out var missingColumns);

            return !checkHeaders ? (false, missingColumns) : (checkHeaders, []);
        }
        catch (InvalidOperationException)
        {
            return (false, []);
        }
    }

    public CsvParseResult<SubmissionDetail> Parse(string filePath)
    {
        var rejected = new List<CsvParseError>();
        var options = new CsvReaderOptions
        {
            OnRecordError = error =>
            {
                rejected.Add(new CsvParseError((int)error.RowNumber, error.RawRecord, error.Exception.Message));
                return true;
            }
        };

        using var reader = CsvReader.FromFile(filePath, options);
        var submissions = reader.GetRecords<SubmissionDetail>().ToList();

        return new CsvParseResult<SubmissionDetail>(submissions, rejected);
    }
}