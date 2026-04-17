using Exporter.Core.Builders;
using Exporter.Csv;

namespace Exporter.Tests;

public class CsvProviderTests
{
    [Fact]
    public void ToCsv_ProducesValidCsv()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .AddColumn(x => x.Department, col => col.Header("Departman"))
            .ToCsv();

        var bytes = result.Export();
        var csv = System.Text.Encoding.UTF8.GetString(bytes);

        Assert.Contains("Ad,Departman", csv);
        Assert.Contains("Ahmet Yılmaz,Yazılım", csv);
        Assert.Contains("Ayşe Demir,Pazarlama", csv);
    }

    [Fact]
    public void ToCsv_WithSemicolonDelimiter_UsesSemicolon()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .ToCsv(opt => opt.Delimiter = ";");

        var csv = System.Text.Encoding.UTF8.GetString(result.Export());

        Assert.Contains("Ad;Maaş", csv);
    }

    [Fact]
    public void ToCsv_WithoutHeader_OmitsHeaderRow()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .ToCsv(opt => opt.IncludeHeader = false);

        var csv = System.Text.Encoding.UTF8.GetString(result.Export());

        Assert.DoesNotContain("Ad", csv);
        Assert.Contains("Ahmet Yılmaz", csv);
    }

    [Fact]
    public void ToCsv_EscapesFieldsWithCommas()
    {
        var employees = new List<Employee>
        {
            new() { Name = "Yılmaz, Ahmet", Department = "IT", Email = "a@b.c", Salary = 1000, BirthDate = DateTime.Now }
        };

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .ToCsv();

        var csv = System.Text.Encoding.UTF8.GetString(result.Export());

        Assert.Contains("\"Yılmaz, Ahmet\"", csv);
    }

    [Fact]
    public void ToCsv_MultiSheet_IncludesSheetSeparators()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .Sheet("Sayfa2")
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .ToCsv();

        var csv = System.Text.Encoding.UTF8.GetString(result.Export());

        Assert.Contains("--- Sheet1 ---", csv);
        Assert.Contains("--- Sayfa2 ---", csv);
    }
}
