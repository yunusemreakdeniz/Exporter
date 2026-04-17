using Exporter.Core.Models;

namespace Exporter.Core.Validation;

public static class ValidationEngine
{
    public static ValidationResult Validate<T>(ExportConfiguration<T> configuration) where T : class
    {
        var result = new ValidationResult();
        var dataList = configuration.Data.ToList();

        foreach (var sheet in configuration.Sheets)
        {
            foreach (var column in sheet.Columns)
            {
                if (column.ValidationRules.Count == 0)
                    continue;

                for (var rowIndex = 0; rowIndex < dataList.Count; rowIndex++)
                {
                    var value = column.GetValue(dataList[rowIndex]);

                    foreach (var rule in column.ValidationRules)
                    {
                        if (rule.IsValid(value))
                            continue;

                        result.Errors.Add(new ValidationError
                        {
                            RowIndex = rowIndex,
                            ColumnName = column.Header,
                            Value = value,
                            Message = rule.ErrorMessage
                        });
                    }
                }
            }
        }

        return result;
    }
}
