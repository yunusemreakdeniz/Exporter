using Exporter.Core.Models;

namespace Exporter.Core.Builders;

public class ColumnBuilder<T, TProp> where T : class
{
    private readonly ColumnDefinition<T> _column;

    internal ColumnBuilder(ColumnDefinition<T> column)
    {
        _column = column;
    }

    public ColumnBuilder<T, TProp> Header(string header)
    {
        _column.Header = header;
        return this;
    }

    public ColumnBuilder<T, TProp> Format(string format)
    {
        _column.Format = format;
        return this;
    }

    public ColumnBuilder<T, TProp> Width(int width)
    {
        _column.Width = width;
        return this;
    }

    public ColumnBuilder<T, TProp> Order(int order)
    {
        _column.Order = order;
        return this;
    }

    public ColumnBuilder<T, TProp> Style(Action<StyleBuilder> configure)
    {
        var builder = new StyleBuilder();
        configure(builder);
        _column.Style = builder.Build();
        return this;
    }

    public ColumnBuilder<T, TProp> Validate(Action<ValidationBuilder> configure)
    {
        var builder = new ValidationBuilder();
        configure(builder);
        _column.ValidationRules = builder.Build();
        return this;
    }

    public ColumnBuilder<T, TProp> Transform(Func<TProp, object> transformer)
    {
        _column.Transformer = raw => raw is TProp typed ? transformer(typed) : raw;
        return this;
    }
}
