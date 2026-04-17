using Exporter.Core.Abstractions;
using Exporter.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Exporter.Pdf;

public class PdfExportProvider : IExportProvider
{
    public byte[] Export<T>(ExportConfiguration<T> configuration) where T : class
    {
        using var stream = new MemoryStream();
        Export(configuration, stream);
        return stream.ToArray();
    }

    public void Export<T>(ExportConfiguration<T> configuration, Stream stream) where T : class
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var opts = GetOption<T, PdfOptions>(configuration, "PdfOptions") ?? new PdfOptions();
        var theme = opts.Theme;
        var data = configuration.Data.ToList();
        var totalRecords = data.Count;

        var pageSize = opts.PageOrientation == PageOrientation.Landscape
            ? PageSizes.A4.Landscape()
            : PageSizes.A4;

        var document = Document.Create(container =>
        {
            // --- Cover Page ---
            if (opts.ShowCoverPage)
                BuildCoverPage(container, opts, theme, totalRecords);

            // --- Data Pages ---
            foreach (var sheet in configuration.Sheets)
            {
                container.Page(page =>
                {
                    page.Size(pageSize);
                    page.MarginVertical(30);
                    page.MarginHorizontal(35);
                    page.DefaultTextStyle(x => x.FontSize(opts.FontSize).FontColor(Color.FromHex(theme.BodyFontColor)));

                    // --- Page Header: Company + Summary ---
                    page.Header().Element(headerContainer =>
                    {
                        headerContainer.Column(col =>
                        {
                            col.Item().Element(c => BuildCompanyHeader(c, opts, theme));

                            if (opts.ShowSummary)
                            {
                                col.Item().PaddingTop(6).Element(c => BuildSummaryBar(c, opts, theme, sheet.Name, totalRecords));
                            }

                            col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(Color.FromHex(theme.AccentColor));
                        });
                    });

                    // --- Content: Table ---
                    page.Content().PaddingTop(6).Element(contentContainer =>
                    {
                        if (!string.IsNullOrEmpty(opts.Watermark))
                        {
                            contentContainer.Layers(layers =>
                            {
                                layers.PrimaryLayer().Element(c => BuildDataTable(c, sheet, data, opts, theme));
                                layers.Layer().AlignCenter().AlignMiddle().Element(c => BuildWatermark(c, opts.Watermark!));
                            });
                        }
                        else
                        {
                            BuildDataTable(contentContainer, sheet, data, opts, theme);
                        }
                    });

                    // --- Footer ---
                    page.Footer().Element(c => BuildFooter(c, opts, theme));
                });
            }
        });

        document.GeneratePdf(stream);
    }

    // ─────────────────────────────────────────────────
    // COVER PAGE
    // ─────────────────────────────────────────────────
    private static void BuildCoverPage(IDocumentContainer container, PdfOptions opts, PdfColorTheme theme, int totalRecords)
    {
        container.Page(page =>
        {
            page.Size(opts.PageOrientation == PageOrientation.Landscape
                ? PageSizes.A4.Landscape()
                : PageSizes.A4);
            page.Margin(0);

            page.Content().Column(col =>
            {
                // Top color band
                col.Item().Height(220).Background(Color.FromHex(theme.CoverBackground)).AlignCenter().AlignMiddle().Column(inner =>
                {
                    inner.Item().AlignCenter().PaddingTop(40).Text(opts.CompanyName ?? "")
                        .FontSize(28).Bold().FontColor(Color.FromHex(theme.CoverFontColor));

                    if (!string.IsNullOrEmpty(opts.LogoPath) && File.Exists(opts.LogoPath))
                    {
                        inner.Item().AlignCenter().PaddingTop(10).Height(60).Image(opts.LogoPath).FitHeight();
                    }
                });

                // Spacer
                col.Item().Height(80);

                // Title block
                col.Item().AlignCenter().PaddingHorizontal(60).Column(inner =>
                {
                    if (!string.IsNullOrEmpty(opts.Title))
                    {
                        inner.Item().AlignCenter().Text(opts.Title)
                            .FontSize(32).Bold().FontColor(Color.FromHex(theme.AccentColor));
                    }

                    if (!string.IsNullOrEmpty(opts.Subtitle))
                    {
                        inner.Item().AlignCenter().PaddingTop(12).Text(opts.Subtitle)
                            .FontSize(16).FontColor(Color.FromHex("#666666"));
                    }

                    // Decorative line
                    inner.Item().PaddingVertical(20).AlignCenter().Width(120).LineHorizontal(3).LineColor(Color.FromHex(theme.AccentColor));

                    // Date + record count
                    inner.Item().AlignCenter().Text(text =>
                    {
                        text.Span(DateTime.Now.ToString("dd MMMM yyyy"))
                            .FontSize(12).FontColor(Color.FromHex("#888888"));
                    });

                    inner.Item().AlignCenter().PaddingTop(4).Text(text =>
                    {
                        text.Span($"Toplam {totalRecords} kayıt")
                            .FontSize(11).FontColor(Color.FromHex("#AAAAAA"));
                    });
                });

                // Bottom accent bar
                col.Item().ExtendVertical();
                col.Item().Height(8).Background(Color.FromHex(theme.AccentColor));
            });
        });
    }

    // ─────────────────────────────────────────────────
    // COMPANY HEADER (every data page)
    // ─────────────────────────────────────────────────
    private static void BuildCompanyHeader(IContainer container, PdfOptions opts, PdfColorTheme theme)
    {
        container.Row(row =>
        {
            if (!string.IsNullOrEmpty(opts.LogoPath) && File.Exists(opts.LogoPath))
            {
                row.ConstantItem(40).Height(40).Image(opts.LogoPath).FitArea();
                row.ConstantItem(10);
            }

            row.RelativeItem().Column(col =>
            {
                if (!string.IsNullOrEmpty(opts.CompanyName))
                {
                    col.Item().Text(opts.CompanyName)
                        .FontSize(14).Bold().FontColor(Color.FromHex(theme.AccentColor));
                }

                if (!string.IsNullOrEmpty(opts.Title))
                {
                    col.Item().Text(opts.Title)
                        .FontSize(11).FontColor(Color.FromHex("#555555"));
                }
            });

            row.RelativeItem().AlignRight().AlignBottom().Text(DateTime.Now.ToString("dd.MM.yyyy"))
                .FontSize(9).FontColor(Color.FromHex(theme.FooterFontColor));
        });
    }

    // ─────────────────────────────────────────────────
    // SUMMARY BAR
    // ─────────────────────────────────────────────────
    private static void BuildSummaryBar(IContainer container, PdfOptions opts, PdfColorTheme theme, string sheetName, int totalRecords)
    {
        container.Background(Color.FromHex(theme.SummaryBackground))
            .BorderLeft(3).BorderColor(Color.FromHex(theme.AccentColor))
            .Padding(8)
            .Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span(sheetName).Bold().FontSize(9).FontColor(Color.FromHex(theme.AccentColor));
                    text.Span($"   |   Toplam: {totalRecords} kayıt").FontSize(8).FontColor(Color.FromHex("#666666"));
                    if (!string.IsNullOrEmpty(opts.Subtitle))
                        text.Span($"   |   {opts.Subtitle}").FontSize(8).FontColor(Color.FromHex("#888888"));
                });
            });
    }

    // ─────────────────────────────────────────────────
    // DATA TABLE
    // ─────────────────────────────────────────────────
    private static void BuildDataTable<T>(IContainer container, SheetDefinition<T> sheet, List<T> data, PdfOptions opts, PdfColorTheme theme) where T : class
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                foreach (var col in sheet.Columns)
                {
                    if (col.Width.HasValue)
                        columns.ConstantColumn(col.Width.Value);
                    else
                        columns.RelativeColumn();
                }
            });

            // --- Table Header ---
            table.Header(header =>
            {
                var isMinimalTheme = theme.HeaderBackground == "#FFFFFF";

                foreach (var col in sheet.Columns)
                {
                    var cell = header.Cell()
                        .Background(Color.FromHex(theme.HeaderBackground))
                        .BorderBottom(isMinimalTheme ? 2f : 0.5f)
                        .BorderColor(Color.FromHex(isMinimalTheme ? theme.AccentColor : theme.HeaderBackground))
                        .Padding(7);

                    var alignment = ResolveAlignment(col.Style?.Alignment);

                    var aligned = alignment switch
                    {
                        Core.Models.Alignment.Right => cell.AlignRight(),
                        Core.Models.Alignment.Center => cell.AlignCenter(),
                        _ => cell.AlignLeft()
                    };

                    aligned.Text(col.Header)
                        .FontColor(Color.FromHex(theme.HeaderFontColor))
                        .Bold()
                        .FontSize(opts.FontSize);
                }
            });

            // --- Data Rows ---
            for (var rowIndex = 0; rowIndex < data.Count; rowIndex++)
            {
                var item = data[rowIndex];
                var isAlternate = rowIndex % 2 == 1;

                foreach (var col in sheet.Columns)
                {
                    var cellBase = table.Cell();
                    IContainer inner;

                    if (opts.ZebraRows && isAlternate)
                        inner = cellBase.Background(Color.FromHex(theme.AlternateRowColor));
                    else
                        inner = cellBase;

                    var styled = inner
                        .BorderBottom(0.5f)
                        .BorderColor(Color.FromHex(theme.BorderColor))
                        .PaddingVertical(5)
                        .PaddingHorizontal(7);

                    var alignment = ResolveAlignment(col.Style?.Alignment);
                    if (alignment == null)
                        alignment = IsNumericValue(col.GetValue(item)) ? Core.Models.Alignment.Right : null;

                    var aligned = alignment switch
                    {
                        Core.Models.Alignment.Right => styled.AlignRight(),
                        Core.Models.Alignment.Center => styled.AlignCenter(),
                        _ => styled.AlignLeft()
                    };

                    var displayValue = col.GetFormattedValue(item);
                    var textDescriptor = aligned.Text(displayValue).FontSize(opts.FontSize);

                    if (col.Style != null)
                        ApplyTextStyle(textDescriptor, col.Style, theme);
                }
            }
        });
    }

    // ─────────────────────────────────────────────────
    // WATERMARK
    // ─────────────────────────────────────────────────
    private static void BuildWatermark(IContainer container, string text)
    {
        container
            .TranslateY(-20)
            .Rotate(-35)
            .Text(text)
            .FontSize(60)
            .Bold()
            .FontColor(Color.FromHex("#DDDDDD"));
    }

    // ─────────────────────────────────────────────────
    // FOOTER
    // ─────────────────────────────────────────────────
    private static void BuildFooter(IContainer container, PdfOptions opts, PdfColorTheme theme)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Color.FromHex(theme.BorderColor));

            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(text =>
                {
                    if (!string.IsNullOrEmpty(opts.FooterNote))
                    {
                        text.Span(opts.FooterNote).FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
                    }
                    else if (!string.IsNullOrEmpty(opts.CompanyName))
                    {
                        text.Span(opts.CompanyName).FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
                    }
                });

                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("Sayfa ").FontSize(8).FontColor(Color.FromHex(theme.FooterFontColor));
                    text.CurrentPageNumber().FontSize(8).Bold().FontColor(Color.FromHex(theme.AccentColor));
                    text.Span(" / ").FontSize(8).FontColor(Color.FromHex(theme.FooterFontColor));
                    text.TotalPages().FontSize(8).Bold().FontColor(Color.FromHex(theme.AccentColor));
                });

                row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                    .FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
            });
        });
    }

    // ─────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────
    private static void ApplyTextStyle(TextSpanDescriptor descriptor, StyleDefinition style, PdfColorTheme theme)
    {
        if (style.IsBold)
            descriptor.Bold();
        if (style.IsItalic)
            descriptor.Italic();
        if (style.FontSize.HasValue)
            descriptor.FontSize(style.FontSize.Value);
        if (!string.IsNullOrEmpty(style.FontColor))
            descriptor.FontColor(Color.FromHex(style.FontColor));
        if (!string.IsNullOrEmpty(style.BackgroundColor))
            descriptor.BackgroundColor(Color.FromHex(style.BackgroundColor));
    }

    private static Core.Models.Alignment? ResolveAlignment(Core.Models.Alignment? explicit_)
    {
        return explicit_;
    }

    private static bool IsNumericValue(object? value)
    {
        return value is int or long or float or double or decimal;
    }

    private static TOption? GetOption<T, TOption>(ExportConfiguration<T> configuration, string key) where T : class
    {
        if (configuration.ProviderOptions.TryGetValue(key, out var value) && value is TOption typed)
            return typed;
        return default;
    }
}
