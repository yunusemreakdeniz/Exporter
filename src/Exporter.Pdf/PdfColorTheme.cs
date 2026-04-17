namespace Exporter.Pdf;

public class PdfColorTheme
{
    public string HeaderBackground { get; init; } = "#2F5496";
    public string HeaderFontColor { get; init; } = "#FFFFFF";
    public string AlternateRowColor { get; init; } = "#F2F2F2";
    public string AccentColor { get; init; } = "#2F5496";
    public string CoverBackground { get; init; } = "#2F5496";
    public string CoverFontColor { get; init; } = "#FFFFFF";
    public string BorderColor { get; init; } = "#D9D9D9";
    public string BodyFontColor { get; init; } = "#333333";
    public string FooterFontColor { get; init; } = "#888888";
    public string SummaryBackground { get; init; } = "#E8EDF4";
}

public static class PdfThemes
{
    public static PdfColorTheme Blue => new()
    {
        HeaderBackground = "#2F5496",
        HeaderFontColor = "#FFFFFF",
        AlternateRowColor = "#E8EDF4",
        AccentColor = "#2F5496",
        CoverBackground = "#2F5496",
        CoverFontColor = "#FFFFFF",
        BorderColor = "#B4C6E7",
        BodyFontColor = "#1F1F1F",
        FooterFontColor = "#7F7F7F",
        SummaryBackground = "#D6E4F0"
    };

    public static PdfColorTheme Green => new()
    {
        HeaderBackground = "#548235",
        HeaderFontColor = "#FFFFFF",
        AlternateRowColor = "#EBF1DE",
        AccentColor = "#548235",
        CoverBackground = "#548235",
        CoverFontColor = "#FFFFFF",
        BorderColor = "#C5E0B4",
        BodyFontColor = "#1F1F1F",
        FooterFontColor = "#7F7F7F",
        SummaryBackground = "#D8E4BC"
    };

    public static PdfColorTheme Red => new()
    {
        HeaderBackground = "#C00000",
        HeaderFontColor = "#FFFFFF",
        AlternateRowColor = "#FCE4EC",
        AccentColor = "#C00000",
        CoverBackground = "#C00000",
        CoverFontColor = "#FFFFFF",
        BorderColor = "#F4B6C2",
        BodyFontColor = "#1F1F1F",
        FooterFontColor = "#7F7F7F",
        SummaryBackground = "#F8D7DA"
    };

    public static PdfColorTheme Dark => new()
    {
        HeaderBackground = "#2D2D2D",
        HeaderFontColor = "#F0F0F0",
        AlternateRowColor = "#F5F5F5",
        AccentColor = "#2D2D2D",
        CoverBackground = "#1A1A1A",
        CoverFontColor = "#F0F0F0",
        BorderColor = "#CCCCCC",
        BodyFontColor = "#1F1F1F",
        FooterFontColor = "#999999",
        SummaryBackground = "#E8E8E8"
    };

    public static PdfColorTheme Minimal => new()
    {
        HeaderBackground = "#FFFFFF",
        HeaderFontColor = "#1F1F1F",
        AlternateRowColor = "#FAFAFA",
        AccentColor = "#444444",
        CoverBackground = "#FFFFFF",
        CoverFontColor = "#1F1F1F",
        BorderColor = "#E0E0E0",
        BodyFontColor = "#333333",
        FooterFontColor = "#AAAAAA",
        SummaryBackground = "#F5F5F5"
    };
}
