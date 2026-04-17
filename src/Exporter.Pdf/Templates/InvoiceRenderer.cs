using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Exporter.Pdf.Templates;

public static class InvoiceRenderer
{
    public static byte[] Render(InvoiceData data)
    {
        using var stream = new MemoryStream();
        Render(data, stream);
        return stream.ToArray();
    }

    public static void Render(InvoiceData data, Stream stream)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var theme = data.Theme;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginHorizontal(40);
                page.MarginVertical(35);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(Color.FromHex(theme.BodyFontColor)));

                page.Header().Element(c => RenderHeader(c, data, theme));

                page.Content().Element(c => RenderContent(c, data, theme));

                page.Footer().Element(c => RenderFooter(c, data, theme));
            });
        });

        document.GeneratePdf(stream);
    }

    // ─────────────────────────────────────────────────
    // HEADER: Company info (left) + Invoice meta (right)
    // ─────────────────────────────────────────────────
    private static void RenderHeader(IContainer container, InvoiceData data, PdfColorTheme theme)
    {
        container.Column(col =>
        {
            // Accent bar at top
            col.Item().Height(4).Background(Color.FromHex(theme.AccentColor));

            col.Item().PaddingTop(15).Row(row =>
            {
                // Left: Company
                row.RelativeItem(3).Column(left =>
                {
                    if (!string.IsNullOrEmpty(data.Company.LogoPath) && File.Exists(data.Company.LogoPath))
                    {
                        left.Item().Height(45).Width(120).Image(data.Company.LogoPath).FitArea();
                        left.Item().PaddingTop(6);
                    }

                    left.Item().Text(data.Company.Name)
                        .FontSize(16).Bold().FontColor(Color.FromHex(theme.AccentColor));

                    if (!string.IsNullOrEmpty(data.Company.Address))
                        left.Item().PaddingTop(2).Text(data.Company.Address).FontSize(8.5f);

                    if (!string.IsNullOrEmpty(data.Company.Phone))
                        left.Item().PaddingTop(1).Text($"Tel: {data.Company.Phone}").FontSize(8.5f);

                    if (!string.IsNullOrEmpty(data.Company.Email))
                        left.Item().PaddingTop(1).Text(data.Company.Email).FontSize(8.5f);

                    if (!string.IsNullOrEmpty(data.Company.TaxOffice) || !string.IsNullOrEmpty(data.Company.TaxNumber))
                    {
                        var taxLine = $"VD: {data.Company.TaxOffice ?? ""} / {data.Company.TaxNumber ?? ""}";
                        left.Item().PaddingTop(1).Text(taxLine.Trim()).FontSize(8.5f);
                    }
                });

                // Right: Invoice meta
                row.RelativeItem(2).AlignRight().Column(right =>
                {
                    right.Item().AlignRight().Text("FATURA")
                        .FontSize(28).Bold().FontColor(Color.FromHex(theme.AccentColor));

                    right.Item().PaddingTop(10).AlignRight().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn();
                            c.ConstantColumn(10);
                            c.RelativeColumn();
                        });

                        AddMetaRow(table, "Fatura No", data.Info.InvoiceNumber, theme);
                        AddMetaRow(table, "Tarih", data.Info.Date.ToString("dd.MM.yyyy"), theme);
                        if (data.Info.DueDate.HasValue)
                            AddMetaRow(table, "Vade", data.Info.DueDate.Value.ToString("dd.MM.yyyy"), theme);
                    });
                });
            });

            // Separator
            col.Item().PaddingTop(12).LineHorizontal(1).LineColor(Color.FromHex(theme.BorderColor));
        });
    }

    private static void AddMetaRow(TableDescriptor table, string label, string value, PdfColorTheme theme)
    {
        table.Cell().AlignRight().PaddingVertical(2).Text(label).FontSize(8.5f).Bold()
            .FontColor(Color.FromHex(theme.AccentColor));
        table.Cell().AlignCenter().PaddingVertical(2).Text(":").FontSize(8.5f);
        table.Cell().AlignLeft().PaddingVertical(2).Text(value).FontSize(8.5f);
    }

    // ─────────────────────────────────────────────────
    // CONTENT: Customer + Items + Totals + Notes + Bank
    // ─────────────────────────────────────────────────
    private static void RenderContent(IContainer container, InvoiceData data, PdfColorTheme theme)
    {
        container.PaddingTop(8).Column(col =>
        {
            // Customer block
            col.Item().Element(c => RenderCustomerBlock(c, data.Customer, theme));

            // Items table
            col.Item().PaddingTop(15).Element(c => RenderItemsTable(c, data, theme));

            // Totals
            col.Item().PaddingTop(2).Element(c => RenderTotals(c, data, theme));

            // Notes
            if (!string.IsNullOrEmpty(data.Notes))
                col.Item().PaddingTop(20).Element(c => RenderNotes(c, data.Notes, theme));

            // Bank info
            if (data.Bank != null)
                col.Item().PaddingTop(12).Element(c => RenderBankInfo(c, data.Bank, theme));
        });
    }

    // ─────────────────────────────────────────────────
    // CUSTOMER BLOCK
    // ─────────────────────────────────────────────────
    private static void RenderCustomerBlock(IContainer container, CustomerInfo customer, PdfColorTheme theme)
    {
        container
            .BorderLeft(3).BorderColor(Color.FromHex(theme.AccentColor))
            .Background(Color.FromHex(theme.SummaryBackground))
            .Padding(10)
            .Column(col =>
            {
                col.Item().Text("MÜŞTERİ").FontSize(8).Bold()
                    .FontColor(Color.FromHex(theme.AccentColor)).LetterSpacing(0.05f);

                col.Item().PaddingTop(4).Text(customer.Name).FontSize(10).Bold();

                if (!string.IsNullOrEmpty(customer.Address))
                    col.Item().PaddingTop(1).Text(customer.Address).FontSize(8.5f);

                if (!string.IsNullOrEmpty(customer.Phone))
                    col.Item().PaddingTop(1).Text($"Tel: {customer.Phone}").FontSize(8.5f);

                if (!string.IsNullOrEmpty(customer.Email))
                    col.Item().PaddingTop(1).Text(customer.Email).FontSize(8.5f);

                if (!string.IsNullOrEmpty(customer.TaxOffice) || !string.IsNullOrEmpty(customer.TaxNumber))
                {
                    var taxLine = $"VD: {customer.TaxOffice ?? ""} / {customer.TaxNumber ?? ""}";
                    col.Item().PaddingTop(1).Text(taxLine.Trim()).FontSize(8.5f);
                }
            });
    }

    // ─────────────────────────────────────────────────
    // ITEMS TABLE
    // ─────────────────────────────────────────────────
    private static void RenderItemsTable(IContainer container, InvoiceData data, PdfColorTheme theme)
    {
        var currency = data.Info.Currency ?? "₺";

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);   // #
                columns.RelativeColumn(4);    // Description
                columns.ConstantColumn(55);   // Quantity
                columns.ConstantColumn(85);   // Unit Price
                columns.ConstantColumn(50);   // Tax %
                columns.ConstantColumn(95);   // Total
            });

            var headerBg = Color.FromHex(theme.HeaderBackground);
            var headerFg = Color.FromHex(theme.HeaderFontColor);
            var bodyFg = Color.FromHex(theme.BodyFontColor);

            // Header
            table.Header(header =>
            {
                StyledCell(header.Cell(), "#", headerBg, headerFg, theme, HAlign.Center, isHeader: true);
                StyledCell(header.Cell(), "Açıklama", headerBg, headerFg, theme, HAlign.Left, isHeader: true);
                StyledCell(header.Cell(), "Miktar", headerBg, headerFg, theme, HAlign.Center, isHeader: true);
                StyledCell(header.Cell(), $"Birim Fiyat ({currency})", headerBg, headerFg, theme, HAlign.Right, isHeader: true);
                StyledCell(header.Cell(), "KDV %", headerBg, headerFg, theme, HAlign.Center, isHeader: true);
                StyledCell(header.Cell(), $"Toplam ({currency})", headerBg, headerFg, theme, HAlign.Right, isHeader: true);
            });

            // Rows
            for (var i = 0; i < data.Items.Count; i++)
            {
                var item = data.Items[i];
                var isAlt = i % 2 == 1;
                var bgColor = isAlt ? Color.FromHex(theme.AlternateRowColor) : Colors.White;

                StyledCell(table.Cell(), $"{i + 1}", bgColor, bodyFg, theme, HAlign.Center);
                StyledCell(table.Cell(), item.Description, bgColor, bodyFg, theme, HAlign.Left);
                StyledCell(table.Cell(), item.Quantity.ToString("G"), bgColor, bodyFg, theme, HAlign.Center);
                StyledCell(table.Cell(), item.UnitPrice.ToString("N2"), bgColor, bodyFg, theme, HAlign.Right);
                StyledCell(table.Cell(), $"%{item.TaxRate:G}", bgColor, bodyFg, theme, HAlign.Center);
                StyledCell(table.Cell(), item.LineTotal.ToString("N2"), bgColor, bodyFg, theme, HAlign.Right, bold: true);
            }
        });
    }

    private static void StyledCell(IContainer cell, string text, Color bgColor, Color fontColor, PdfColorTheme theme, HAlign align, bool bold = false, bool isHeader = false)
    {
        IContainer styled = cell.Background(bgColor);

        if (!isHeader)
            styled = styled.BorderBottom(0.5f).BorderColor(Color.FromHex(theme.BorderColor));

        styled = styled.PaddingVertical(isHeader ? 7 : 6).PaddingHorizontal(6);

        var aligned = align switch
        {
            HAlign.Right => styled.AlignRight(),
            HAlign.Center => styled.AlignCenter(),
            _ => styled.AlignLeft()
        };

        var td = aligned.Text(text).FontSize(8.5f).FontColor(fontColor);
        if (bold || isHeader) td.Bold();
    }

    private enum HAlign { Left, Center, Right }

    // ─────────────────────────────────────────────────
    // TOTALS BLOCK
    // ─────────────────────────────────────────────────
    private static void RenderTotals(IContainer container, InvoiceData data, PdfColorTheme theme)
    {
        var currency = data.Info.Currency ?? "₺";

        // Group items by tax rate for breakdown
        var taxGroups = data.Items
            .GroupBy(i => i.TaxRate)
            .OrderBy(g => g.Key)
            .ToList();

        container.AlignRight().Width(250).Column(col =>
        {
            col.Item().BorderTop(1).BorderColor(Color.FromHex(theme.BorderColor));

            // Subtotal
            TotalRow(col, "Ara Toplam", data.Subtotal.ToString("N2"), currency, theme);

            // Tax breakdown per rate
            foreach (var group in taxGroups)
            {
                var taxTotal = group.Sum(i => i.TaxAmount);
                TotalRow(col, $"KDV (%{group.Key:G})", taxTotal.ToString("N2"), currency, theme);
            }

            // Grand total
            col.Item().PaddingTop(2).BorderTop(2).BorderColor(Color.FromHex(theme.AccentColor));
            col.Item().PaddingVertical(6).PaddingHorizontal(8)
                .Background(Color.FromHex(theme.SummaryBackground))
                .Row(row =>
                {
                    row.RelativeItem().AlignLeft().Text("GENEL TOPLAM").FontSize(11).Bold()
                        .FontColor(Color.FromHex(theme.AccentColor));
                    row.RelativeItem().AlignRight().Text($"{data.GrandTotal:N2} {currency}")
                        .FontSize(11).Bold().FontColor(Color.FromHex(theme.AccentColor));
                });
        });
    }

    private static void TotalRow(ColumnDescriptor col, string label, string value, string currency, PdfColorTheme theme)
    {
        col.Item().PaddingVertical(3).PaddingHorizontal(8).Row(row =>
        {
            row.RelativeItem().AlignLeft().Text(label).FontSize(9)
                .FontColor(Color.FromHex("#555555"));
            row.RelativeItem().AlignRight().Text($"{value} {currency}").FontSize(9);
        });
    }

    // ─────────────────────────────────────────────────
    // NOTES
    // ─────────────────────────────────────────────────
    private static void RenderNotes(IContainer container, string notes, PdfColorTheme theme)
    {
        container
            .BorderLeft(3).BorderColor(Color.FromHex(theme.FooterFontColor))
            .Background(Color.FromHex("#F9F9F9"))
            .Padding(10)
            .Column(col =>
            {
                col.Item().Text("NOTLAR").FontSize(8).Bold()
                    .FontColor(Color.FromHex(theme.FooterFontColor)).LetterSpacing(0.05f);
                col.Item().PaddingTop(4).Text(notes).FontSize(8.5f)
                    .FontColor(Color.FromHex("#555555"));
            });
    }

    // ─────────────────────────────────────────────────
    // BANK INFO
    // ─────────────────────────────────────────────────
    private static void RenderBankInfo(IContainer container, BankDetails bank, PdfColorTheme theme)
    {
        container
            .BorderLeft(3).BorderColor(Color.FromHex(theme.AccentColor))
            .Background(Color.FromHex(theme.SummaryBackground))
            .Padding(10)
            .Column(col =>
            {
                col.Item().Text("BANKA BİLGİLERİ").FontSize(8).Bold()
                    .FontColor(Color.FromHex(theme.AccentColor)).LetterSpacing(0.05f);

                col.Item().PaddingTop(4).Text(bank.BankName).FontSize(9).Bold();

                col.Item().PaddingTop(2).Row(row =>
                {
                    row.AutoItem().Text("IBAN: ").FontSize(8.5f).Bold();
                    row.AutoItem().Text(bank.Iban).FontSize(8.5f);
                });

                if (!string.IsNullOrEmpty(bank.AccountHolder))
                    col.Item().PaddingTop(1).Text($"Hesap Sahibi: {bank.AccountHolder}").FontSize(8.5f);
            });
    }

    // ─────────────────────────────────────────────────
    // FOOTER
    // ─────────────────────────────────────────────────
    private static void RenderFooter(IContainer container, InvoiceData data, PdfColorTheme theme)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Color.FromHex(theme.BorderColor));
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().AlignLeft().Text(data.Company.Name)
                    .FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));

                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("Sayfa ").FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
                    text.CurrentPageNumber().FontSize(7).Bold().FontColor(Color.FromHex(theme.AccentColor));
                    text.Span(" / ").FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
                    text.TotalPages().FontSize(7).Bold().FontColor(Color.FromHex(theme.AccentColor));
                });

                row.RelativeItem().AlignRight().Text($"Oluşturma: {DateTime.Now:dd.MM.yyyy HH:mm}")
                    .FontSize(7).FontColor(Color.FromHex(theme.FooterFontColor));
            });
        });
    }
}
