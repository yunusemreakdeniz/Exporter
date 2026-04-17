namespace Exporter.Core.Models;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; set; } = [];
}

public class ValidationError
{
    public int RowIndex { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string Message { get; set; } = string.Empty;
}
