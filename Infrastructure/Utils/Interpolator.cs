using System.Text.RegularExpressions;

namespace Infrastructure.Utils;

public partial class Interpolator
{
    [GeneratedRegex(@"\{\{\s*(?<key>\w+)\s*\}\}")]
    private static partial Regex TemplateRegex();

    public static string Resolve(string template, IDictionary<string, string?> variables)
    {
        return TemplateRegex().Replace(template, match =>
        {
            var key = match.Groups["key"].Value;

            return variables.TryGetValue(key, out var value) ? value ?? string.Empty : match.Value;
        });
    }
}