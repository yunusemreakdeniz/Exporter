using Exporter.Core.Models;

namespace Exporter.Core.Builders;

public class StyleBuilder
{
    private readonly StyleDefinition _style = new();

    public StyleBuilder Bold()
    {
        _style.IsBold = true;
        return this;
    }

    public StyleBuilder Italic()
    {
        _style.IsItalic = true;
        return this;
    }

    public StyleBuilder FontSize(int size)
    {
        _style.FontSize = size;
        return this;
    }

    public StyleBuilder FontColor(string hex)
    {
        _style.FontColor = hex;
        return this;
    }

    public StyleBuilder BackgroundColor(string hex)
    {
        _style.BackgroundColor = hex;
        return this;
    }

    public StyleBuilder Alignment(Models.Alignment alignment)
    {
        _style.Alignment = alignment;
        return this;
    }

    public StyleBuilder Border(BorderStyle style)
    {
        _style.Border = style;
        return this;
    }

    internal StyleDefinition Build() => _style;
}
