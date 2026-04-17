using System.Text.RegularExpressions;

namespace Exporter.Core.Validation.Rules;

public class RegexRule(string pattern, string? errorMessage = null) : IValidationRule
{
    private readonly Regex _regex = new(pattern, RegexOptions.Compiled);

    public string ErrorMessage => errorMessage ?? $"Value does not match pattern '{pattern}'.";

    public bool IsValid(object? value)
    {
        if (value is null) return false;
        var str = value.ToString();
        return str is not null && _regex.IsMatch(str);
    }
}
