using Exporter.Core.Builders;
using Exporter.Pdf;

namespace Exporter.Tests;

public class PdfProviderTests
{
    [Fact]
    public void ToPdf_ProducesValidPdfBytes()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad Soyad"))
            .AddColumn(x => x.Department, col => col.Header("Departman"))
            .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("C2"))
            .ToPdf();

        var bytes = result.Export();

        Assert.NotNull(bytes);
        Assert.True(bytes.Length > 0);
        Assert.Equal((byte)'%', bytes[0]);
        Assert.Equal((byte)'P', bytes[1]);
        Assert.Equal((byte)'D', bytes[2]);
        Assert.Equal((byte)'F', bytes[3]);
    }

    [Fact]
    public void ToPdf_MultiSheet_ProducesMultiPagePdf()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .Sheet("Sayfa2")
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .ToPdf();

        var bytes = result.Export();
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void ToPdf_WithTitle_Works()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .ToPdf(opt => opt.Title = "Çalışan Raporu");

        var bytes = result.Export();
        Assert.True(bytes.Length > 0);
    }

    [Fact]
    public void ToPdf_ExportToStream_Works()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name)
            .ToPdf();

        using var stream = result.ExportAsStream();
        Assert.True(stream.Length > 0);
    }
}
