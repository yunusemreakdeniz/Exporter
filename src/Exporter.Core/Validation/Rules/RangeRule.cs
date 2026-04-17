namespace Exporter.Core.Validation.Rules;

public class RangeRule(IComparable min, IComparable max) : IValidationRule
{
    public string ErrorMessage => $"Value must be between {min} and {max}.";

    public bool IsValid(object? value)
    {
        if (value is null) return false;
        if (value is not IComparable comparable) return false;
        return comparable.CompareTo(min) >= 0 && comparable.CompareTo(max) <= 0;
    }
}
