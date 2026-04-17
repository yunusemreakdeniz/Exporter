namespace Exporter.Core.Validation.Rules;

public class MinLengthRule(int minLength) : IValidationRule
{
    public string ErrorMessage => $"Value must be at least {minLength} characters.";

    public bool IsValid(object? value)
    {
        if (value is null) return false;
        return (value.ToString()?.Length ?? 0) >= minLength;
    }
}
