using Exporter.Core.Validation;
using Exporter.Core.Validation.Rules;

namespace Exporter.Core.Builders;

public class ValidationBuilder
{
    private readonly List<IValidationRule> _rules = [];

    public ValidationBuilder Required()
    {
        _rules.Add(new RequiredRule());
        return this;
    }

    public ValidationBuilder MaxLength(int max)
    {
        _rules.Add(new MaxLengthRule(max));
        return this;
    }

    public ValidationBuilder MinLength(int min)
    {
        _rules.Add(new MinLengthRule(min));
        return this;
    }

    public ValidationBuilder Range(IComparable min, IComparable max)
    {
        _rules.Add(new RangeRule(min, max));
        return this;
    }

    public ValidationBuilder Regex(string pattern, string? errorMessage = null)
    {
        _rules.Add(new RegexRule(pattern, errorMessage));
        return this;
    }

    public ValidationBuilder Custom(Func<object?, bool> predicate, string message)
    {
        _rules.Add(new CustomRule(predicate, message));
        return this;
    }

    internal List<IValidationRule> Build() => _rules;
}
