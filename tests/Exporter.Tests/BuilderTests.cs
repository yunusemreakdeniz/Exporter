using Exporter.Core;
using Exporter.Core.Builders;
using Exporter.Core.Models;

namespace Exporter.Tests;

public class BuilderTests
{
    [Fact]
    public void Create_WithData_ReturnsBuilder()
    {
        var employees = TestData.CreateEmployees();
        var builder = ExportBuilder<Employee>.Create(employees);

        Assert.NotNull(builder);
    }

    [Fact]
    public void AddColumn_ExtractsPropertyName()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name)
            .BuildConfiguration();

        var column = config.Sheets[0].Columns[0];
        Assert.Equal("Name", column.PropertyName);
        Assert.Equal("Name", column.Header);
    }

    [Fact]
    public void AddColumn_WithHeader_OverridesDefault()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad Soyad"))
            .BuildConfiguration();

        Assert.Equal("Ad Soyad", config.Sheets[0].Columns[0].Header);
    }

    [Fact]
    public void AddColumn_WithFormat_SetsFormat()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Salary, col => col.Format("C2"))
            .BuildConfiguration();

        Assert.Equal("C2", config.Sheets[0].Columns[0].Format);
    }

    [Fact]
    public void AddColumn_WithStyle_AppliesStyle()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col
                .Style(s => s.Bold().FontColor("#FF0000").FontSize(14)))
            .BuildConfiguration();

        var style = config.Sheets[0].Columns[0].Style;
        Assert.NotNull(style);
        Assert.True(style.IsBold);
        Assert.Equal("#FF0000", style.FontColor);
        Assert.Equal(14, style.FontSize);
    }

    [Fact]
    public void AddColumn_WithWidth_SetsWidth()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Width(30))
            .BuildConfiguration();

        Assert.Equal(30, config.Sheets[0].Columns[0].Width);
    }

    [Fact]
    public void AddColumn_WithOrder_SortsColumns()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Salary, col => col.Header("Maaş").Order(2))
            .AddColumn(x => x.Name, col => col.Header("Ad").Order(1))
            .BuildConfiguration();

        Assert.Equal("Ad", config.Sheets[0].Columns[0].Header);
        Assert.Equal("Maaş", config.Sheets[0].Columns[1].Header);
    }

    [Fact]
    public void AddColumn_WithTransform_TransformsValue()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.IsActive, col => col.Transform(v => v ? "Aktif" : "Pasif"))
            .BuildConfiguration();

        var column = config.Sheets[0].Columns[0];
        var value = column.GetValue(employees[0]);
        Assert.Equal("Aktif", value);

        var value2 = column.GetValue(employees[2]);
        Assert.Equal("Pasif", value2);
    }

    [Fact]
    public void Sheet_CreatesMultipleSheets()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name)
            .Sheet("Detay")
            .AddColumn(x => x.Salary)
            .AddColumn(x => x.Department)
            .BuildConfiguration();

        Assert.Equal(2, config.Sheets.Count);
        Assert.Equal("Sheet1", config.Sheets[0].Name);
        Assert.Equal("Detay", config.Sheets[1].Name);
        Assert.Single(config.Sheets[0].Columns);
        Assert.Equal(2, config.Sheets[1].Columns.Count);
    }

    [Fact]
    public void HeaderStyle_AppliesStyleToCurrentSheet()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name)
            .HeaderStyle(s => s.Bold().BackgroundColor("#333").FontColor("#FFF"))
            .BuildConfiguration();

        var headerStyle = config.Sheets[0].HeaderStyle;
        Assert.NotNull(headerStyle);
        Assert.True(headerStyle.IsBold);
        Assert.Equal("#333", headerStyle.BackgroundColor);
        Assert.Equal("#FFF", headerStyle.FontColor);
    }

    [Fact]
    public void ColumnDefinition_GetFormattedValue_FormatsCorrectly()
    {
        var employees = TestData.CreateEmployees();
        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Salary, col => col.Format("N2"))
            .AddColumn(x => x.BirthDate, col => col.Format("dd/MM/yyyy"))
            .BuildConfiguration();

        var salaryColumn = config.Sheets[0].Columns[0];
        var dateColumn = config.Sheets[0].Columns[1];

        var salaryFormatted = salaryColumn.GetFormattedValue(employees[0]);
        Assert.Contains("25", salaryFormatted);

        var dateFormatted = dateColumn.GetFormattedValue(employees[0]);
        Assert.Contains("15", dateFormatted);
        Assert.Contains("05", dateFormatted);
        Assert.Contains("1990", dateFormatted);
    }

    [Fact]
    public void StaticExporter_Create_WorksAsShortcut()
    {
        var employees = TestData.CreateEmployees();
        var builder = Core.Exporter.Create(employees);
        Assert.NotNull(builder);
    }
}
