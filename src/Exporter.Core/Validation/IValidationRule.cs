namespace Exporter.Core.Validation;

public interface IValidationRule
{
    string ErrorMessage { get; }
    bool IsValid(object? value);
}
