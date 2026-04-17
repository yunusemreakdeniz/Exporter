using Exporter.Core;
using Exporter.Core.Builders;

namespace Exporter.Pdf;

public static class PdfBuilderExtensions
{
    public static ExportResult<T> ToPdf<T>(this ExportBuilder<T> builder, Action<PdfOptions>? options = null) where T : class
    {
        var provider = new PdfExportProvider();

        if (options != null)
        {
            var opts = new PdfOptions();
            options(opts);
            return builder.To(provider, dict =>
            {
                dict["PdfOptions"] = opts;
            });
        }

        return builder.To(provider);
    }
}

public enum PageOrientation
{
    Portrait,
    Landscape
}

public class PdfOptions
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? CompanyName { get; set; }
    public string? LogoPath { get; set; }
    public PdfColorTheme Theme { get; set; } = PdfThemes.Blue;
    public PageOrientation PageOrientation { get; set; } = PageOrientation.Portrait;
    public bool ShowCoverPage { get; set; }
    public bool ShowSummary { get; set; } = true;
    public string? Watermark { get; set; }
    public bool ZebraRows { get; set; } = true;
    public float FontSize { get; set; } = 9f;
    public string? FooterNote { get; set; }
}
