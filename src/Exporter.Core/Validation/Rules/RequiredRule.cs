namespace Exporter.Core.Validation.Rules;

public class RequiredRule : IValidationRule
{
    public string ErrorMessage => "Value is required.";

    public bool IsValid(object? value)
    {
        return value switch
        {
            null => false,
            string s => !string.IsNullOrWhiteSpace(s),
            _ => true
        };
    }
}
