using System.Text.Json;
using Exporter.Core.Builders;
using Exporter.Json;

namespace Exporter.Tests;

public class JsonProviderTests
{
    [Fact]
    public void ToJson_ProducesValidJson()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .ToJson();

        var bytes = result.Export();
        var json = System.Text.Encoding.UTF8.GetString(bytes);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        Assert.Equal(3, root.GetArrayLength());
        Assert.Equal("Ahmet Yılmaz", root[0].GetProperty("Ad").GetString());
        Assert.Equal(25000m, root[0].GetProperty("Maaş").GetDecimal());
    }

    [Fact]
    public void ToJson_MultiSheet_ProducesNestedObject()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .Sheet("Detay")
            .AddColumn(x => x.Salary, col => col.Header("Maaş"))
            .ToJson();

        var json = System.Text.Encoding.UTF8.GetString(result.Export());
        using var doc = JsonDocument.Parse(json);

        Assert.Equal(JsonValueKind.Object, doc.RootElement.ValueKind);
        Assert.True(doc.RootElement.TryGetProperty("Sheet1", out var sheet1));
        Assert.True(doc.RootElement.TryGetProperty("Detay", out var sheet2));
        Assert.Equal(JsonValueKind.Array, sheet1.ValueKind);
        Assert.Equal(JsonValueKind.Array, sheet2.ValueKind);
    }

    [Fact]
    public void ToJson_WithBooleanAndDate_SerializesCorrectly()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.IsActive, col => col.Header("Aktif"))
            .AddColumn(x => x.BirthDate, col => col.Header("Doğum"))
            .ToJson();

        var json = System.Text.Encoding.UTF8.GetString(result.Export());
        using var doc = JsonDocument.Parse(json);
        var first = doc.RootElement[0];

        Assert.True(first.GetProperty("Aktif").GetBoolean());
        Assert.Equal(JsonValueKind.String, first.GetProperty("Doğum").ValueKind);
    }

    [Fact]
    public void ToJson_Compact_ProducesNonIndented()
    {
        var employees = TestData.CreateEmployees();

        var result = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Header("Ad"))
            .ToJson(opt => opt.Indented = false);

        var json = System.Text.Encoding.UTF8.GetString(result.Export());

        Assert.DoesNotContain("\n", json);
    }
}
