namespace Exporter.Core.Models;

public class ExportConfiguration<T> where T : class
{
    public IEnumerable<T> Data { get; set; } = [];
    public List<SheetDefinition<T>> Sheets { get; set; } = [];
    public Dictionary<string, object> ProviderOptions { get; set; } = [];
}
