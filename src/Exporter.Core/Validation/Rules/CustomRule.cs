namespace Exporter.Core.Validation.Rules;

public class CustomRule(Func<object?, bool> predicate, string errorMessage) : IValidationRule
{
    public string ErrorMessage => errorMessage;

    public bool IsValid(object? value) => predicate(value);
}
