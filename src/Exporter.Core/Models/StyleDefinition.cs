namespace Exporter.Core.Models;

public class StyleDefinition
{
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public string? FontColor { get; set; }
    public string? BackgroundColor { get; set; }
    public int? FontSize { get; set; }
    public Alignment? Alignment { get; set; }
    public BorderStyle? Border { get; set; }

    public StyleDefinition Clone()
    {
        return new StyleDefinition
        {
            IsBold = IsBold,
            IsItalic = IsItalic,
            FontColor = FontColor,
            BackgroundColor = BackgroundColor,
            FontSize = FontSize,
            Alignment = Alignment,
            Border = Border
        };
    }
}
