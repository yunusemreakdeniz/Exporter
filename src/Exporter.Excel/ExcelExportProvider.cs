using ClosedXML.Excel;
using Exporter.Core.Abstractions;
using Exporter.Core.Models;

namespace Exporter.Excel;

public class ExcelExportProvider : IExportProvider
{
    public byte[] Export<T>(ExportConfiguration<T> configuration) where T : class
    {
        using var stream = new MemoryStream();
        Export(configuration, stream);
        return stream.ToArray();
    }

    public void Export<T>(ExportConfiguration<T> configuration, Stream stream) where T : class
    {
        using var workbook = new XLWorkbook();
        var dataList = configuration.Data.ToList();

        foreach (var sheet in configuration.Sheets)
        {
            var worksheet = workbook.Worksheets.Add(sheet.Name);
            WriteHeader(worksheet, sheet);
            WriteData(worksheet, sheet, dataList);
            ApplyColumnWidths(worksheet, sheet);

            if (configuration.ProviderOptions.TryGetValue("AutoFilter", out var af) && af is true)
                worksheet.RangeUsed()?.SetAutoFilter();

            if (configuration.ProviderOptions.TryGetValue("FreezeTopRow", out var ft) && ft is true)
                worksheet.SheetView.FreezeRows(1);
        }

        workbook.SaveAs(stream);
    }

    private static void WriteHeader<T>(IXLWorksheet worksheet, SheetDefinition<T> sheet) where T : class
    {
        for (var col = 0; col < sheet.Columns.Count; col++)
        {
            var column = sheet.Columns[col];
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = column.Header;

            var style = sheet.HeaderStyle ?? CreateDefaultHeaderStyle();
            ApplyCellStyle(cell, style);
        }
    }

    private static void WriteData<T>(IXLWorksheet worksheet, SheetDefinition<T> sheet, List<T> data) where T : class
    {
        for (var row = 0; row < data.Count; row++)
        {
            for (var col = 0; col < sheet.Columns.Count; col++)
            {
                var column = sheet.Columns[col];
                var cell = worksheet.Cell(row + 2, col + 1);
                var value = column.GetValue(data[row]);

                SetCellValue(cell, value, column.Format);

                if (column.Style != null)
                    ApplyCellStyle(cell, column.Style);
            }
        }
    }

    private static void SetCellValue(IXLCell cell, object? value, string? format)
    {
        switch (value)
        {
            case null:
                cell.Value = Blank.Value;
                break;
            case DateTime dt:
                cell.Value = dt;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = format;
                break;
            case DateTimeOffset dto:
                cell.Value = dto.DateTime;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = format;
                break;
            case decimal d:
                cell.Value = (double)d;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = ConvertToExcelFormat(format);
                break;
            case double dbl:
                cell.Value = dbl;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = ConvertToExcelFormat(format);
                break;
            case float f:
                cell.Value = (double)f;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = ConvertToExcelFormat(format);
                break;
            case int i:
                cell.Value = i;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = ConvertToExcelFormat(format);
                break;
            case long l:
                cell.Value = l;
                if (!string.IsNullOrEmpty(format))
                    cell.Style.NumberFormat.Format = ConvertToExcelFormat(format);
                break;
            case bool b:
                cell.Value = b;
                break;
            default:
                cell.Value = value.ToString();
                break;
        }
    }

    private static void ApplyCellStyle(IXLCell cell, StyleDefinition style)
    {
        if (style.IsBold)
            cell.Style.Font.Bold = true;
        if (style.IsItalic)
            cell.Style.Font.Italic = true;
        if (style.FontSize.HasValue)
            cell.Style.Font.FontSize = style.FontSize.Value;
        if (!string.IsNullOrEmpty(style.FontColor))
            cell.Style.Font.FontColor = XLColor.FromHtml(style.FontColor);
        if (!string.IsNullOrEmpty(style.BackgroundColor))
        {
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml(style.BackgroundColor);
            cell.Style.Fill.PatternType = XLFillPatternValues.Solid;
        }
        if (style.Alignment.HasValue)
        {
            cell.Style.Alignment.Horizontal = style.Alignment.Value switch
            {
                Core.Models.Alignment.Left => XLAlignmentHorizontalValues.Left,
                Core.Models.Alignment.Center => XLAlignmentHorizontalValues.Center,
                Core.Models.Alignment.Right => XLAlignmentHorizontalValues.Right,
                _ => XLAlignmentHorizontalValues.General
            };
        }
        if (style.Border.HasValue)
        {
            var borderStyle = style.Border.Value switch
            {
                Core.Models.BorderStyle.Thin => XLBorderStyleValues.Thin,
                Core.Models.BorderStyle.Medium => XLBorderStyleValues.Medium,
                Core.Models.BorderStyle.Thick => XLBorderStyleValues.Thick,
                Core.Models.BorderStyle.Double => XLBorderStyleValues.Double,
                _ => XLBorderStyleValues.None
            };
            cell.Style.Border.TopBorder = borderStyle;
            cell.Style.Border.BottomBorder = borderStyle;
            cell.Style.Border.LeftBorder = borderStyle;
            cell.Style.Border.RightBorder = borderStyle;
        }
    }

    private static void ApplyColumnWidths<T>(IXLWorksheet worksheet, SheetDefinition<T> sheet) where T : class
    {
        for (var col = 0; col < sheet.Columns.Count; col++)
        {
            var column = sheet.Columns[col];
            if (column.Width.HasValue)
                worksheet.Column(col + 1).Width = column.Width.Value;
            else
                worksheet.Column(col + 1).AdjustToContents();
        }
    }

    private static StyleDefinition CreateDefaultHeaderStyle()
    {
        return new StyleDefinition
        {
            IsBold = true,
            BackgroundColor = "#4472C4",
            FontColor = "#FFFFFF"
        };
    }

    private static string ConvertToExcelFormat(string dotNetFormat)
    {
        if (string.IsNullOrEmpty(dotNetFormat))
            return dotNetFormat;

        if (dotNetFormat.StartsWith('C') || dotNetFormat.StartsWith('c'))
        {
            var decimals = dotNetFormat.Length > 1 && int.TryParse(dotNetFormat[1..], out var d) ? d : 2;
            return "#,##0." + new string('0', decimals);
        }

        if (dotNetFormat.StartsWith('N') || dotNetFormat.StartsWith('n'))
        {
            var decimals = dotNetFormat.Length > 1 && int.TryParse(dotNetFormat[1..], out var d) ? d : 2;
            return "#,##0." + new string('0', decimals);
        }

        if (dotNetFormat.StartsWith('P') || dotNetFormat.StartsWith('p'))
        {
            var decimals = dotNetFormat.Length > 1 && int.TryParse(dotNetFormat[1..], out var d) ? d : 2;
            return "0." + new string('0', decimals) + "%";
        }

        return dotNetFormat;
    }
}
