namespace Exporter.Core.Validation.Rules;

public class MaxLengthRule(int maxLength) : IValidationRule
{
    public string ErrorMessage => $"Value must not exceed {maxLength} characters.";

    public bool IsValid(object? value)
    {
        if (value is null) return true;
        return value.ToString()?.Length <= maxLength;
    }
}
