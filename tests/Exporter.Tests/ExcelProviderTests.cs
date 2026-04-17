using ClosedXML.Excel;
using Exporter.Core.Builders;
using Exporter.Excel;

namespace Exporter.Tests;

public class ExcelProviderTests
{
    [Fact]
    public void ToExcel_ProducesValidExcelBytes()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad Soyad"))
            .AddColumn(x => x.Department, col => col.Header("Departman"))
            .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("C2"))
            .ToExcel();

        var bytes = result.Export();

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);

        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        Assert.Equal("Ad Soyad", worksheet.Cell(1, 1).Value.ToString());
        Assert.Equal("Departman", worksheet.Cell(1, 2).Value.ToString());
        Assert.Equal("Maaş", worksheet.Cell(1, 3).Value.ToString());
        Assert.Equal("Ahmet Yılmaz", worksheet.Cell(2, 1).Value.ToString());
        Assert.Equal(3, worksheet.LastRowUsed()!.RowNumber() - 1);
    }

    [Fact]
    public void ToExcel_MultiSheet_CreatesMultipleSheets()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .Sheet("Detay")
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .AddColumn(x => x.Department, col => col.Header("Departman"))
            .ToExcel();

        var bytes = result.Export();

        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);

        Assert.Equal(2, workbook.Worksheets.Count);
        Assert.Equal("Sheet1", workbook.Worksheets.First().Name);
        Assert.Equal("Detay", workbook.Worksheets.Last().Name);
    }

    [Fact]
    public void ToExcel_WithOptions_AppliesOptions()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .ToExcel(opt =>
            {
                opt.AutoFilter = true;
                opt.FreezeTopRow = true;
            });

        var bytes = result.Export();
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void ToExcel_ExportToStream_Works()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name)
            .ToExcel();

        using var stream = result.ExportAsStream();
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void ToExcel_ExportToFile_CreatesFile()
    {
        var employees = TestData.CreateEmployees();
        var tempFile = Path.GetTempFileName() + ".xlsx";

        try
        {
            var result = ExportBuilder<Employee>.Create(employees)
                .AddColumn(x => x.Name, col => col.Header("Ad"))
                .AddColumn(x => x.Salary, col => col.Header("Maaş"))
                .ToExcel();

            result.Export(tempFile);

            Assert.True(File.Exists(tempFile));
            Assert.True(new FileInfo(tempFile).Length > 0);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void ToExcel_WithColumnStyle_AppliesStyle()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col
                .Header("Ad")
                .Style(s => s.Bold().FontColor("#FF0000")))
            .ToExcel();

        var bytes = result.Export();

        using var stream = new MemoryStream(bytes);
        using var workbook = new XLWorkbook(stream);
        var cell = workbook.Worksheets.First().Cell(2, 1);

        Assert.True(cell.Style.Font.Bold);
    }
}
