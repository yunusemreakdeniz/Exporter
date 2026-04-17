using System.Linq.Expressions;
using Exporter.Core.Validation;

namespace Exporter.Core.Models;

public class ColumnDefinition<T> where T : class
{
    public LambdaExpression Expression { get; set; } = null!;
    public Func<T, object?> ValueAccessor { get; set; } = null!;
    public Func<object?, object?>? Transformer { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string Header { get; set; } = string.Empty;
    public string? Format { get; set; }
    public int? Width { get; set; }
    public int Order { get; set; }
    public StyleDefinition? Style { get; set; }
    public List<IValidationRule> ValidationRules { get; set; } = [];

    public object? GetValue(T item)
    {
        var raw = ValueAccessor(item);
        return Transformer != null ? Transformer(raw) : raw;
    }

    public string GetFormattedValue(T item)
    {
        var value = GetValue(item);

        if (value is null)
            return string.Empty;

        if (!string.IsNullOrEmpty(Format) && value is IFormattable formattable)
            return formattable.ToString(Format, null);

        return value.ToString() ?? string.Empty;
    }
}
