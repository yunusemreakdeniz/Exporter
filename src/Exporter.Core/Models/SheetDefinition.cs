namespace Exporter.Core.Models;

public class SheetDefinition<T> where T : class
{
    public string Name { get; set; } = "Sheet1";
    public List<ColumnDefinition<T>> Columns { get; set; } = [];
    public StyleDefinition? HeaderStyle { get; set; }
}
