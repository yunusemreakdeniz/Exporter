using System.Linq.Expressions;
using Exporter.Core.Abstractions;
using Exporter.Core.Models;

namespace Exporter.Core.Builders;

public class ExportBuilder<T> where T : class
{
    private readonly ExportConfiguration<T> _configuration = new();
    private SheetDefinition<T> _currentSheet;

    private ExportBuilder(IEnumerable<T> data)
    {
        _configuration.Data = data;
        _currentSheet = new SheetDefinition<T> { Name = "Sheet1" };
        _configuration.Sheets.Add(_currentSheet);
    }

    public static ExportBuilder<T> Create(IEnumerable<T> data) => new(data);

    public ExportBuilder<T> AddColumn<TProp>(
        Expression<Func<T, TProp>> expression,
        Action<ColumnBuilder<T, TProp>>? configure = null)
    {
        var propertyName = ExtractPropertyName(expression);
        var compiled = expression.Compile();

        var column = new ColumnDefinition<T>
        {
            Expression = expression,
            ValueAccessor = item => compiled(item),
            PropertyName = propertyName,
            Header = propertyName,
            Order = _currentSheet.Columns.Count
        };

        if (configure != null)
        {
            var builder = new ColumnBuilder<T, TProp>(column);
            configure(builder);
        }

        _currentSheet.Columns.Add(column);
        return this;
    }

    public ExportBuilder<T> Sheet(string name)
    {
        _currentSheet = new SheetDefinition<T> { Name = name };
        _configuration.Sheets.Add(_currentSheet);
        return this;
    }

    public ExportBuilder<T> HeaderStyle(Action<StyleBuilder> configure)
    {
        var builder = new StyleBuilder();
        configure(builder);
        _currentSheet.HeaderStyle = builder.Build();
        return this;
    }

    public ExportResult<T> To(IExportProvider provider)
    {
        SortColumns();
        return new ExportResult<T>(provider, _configuration);
    }

    public ExportResult<T> To(IExportProvider provider, Action<Dictionary<string, object>> options)
    {
        var opts = new Dictionary<string, object>();
        options(opts);
        foreach (var kvp in opts)
            _configuration.ProviderOptions[kvp.Key] = kvp.Value;

        SortColumns();
        return new ExportResult<T>(provider, _configuration);
    }

    internal ExportConfiguration<T> BuildConfiguration()
    {
        SortColumns();
        return _configuration;
    }

    private void SortColumns()
    {
        foreach (var sheet in _configuration.Sheets)
            sheet.Columns = [.. sheet.Columns.OrderBy(c => c.Order)];
    }

    private static string ExtractPropertyName(LambdaExpression expression)
    {
        return expression.Body switch
        {
            MemberExpression member => member.Member.Name,
            UnaryExpression { Operand: MemberExpression unaryMember } => unaryMember.Member.Name,
            _ => expression.Body.ToString()
        };
    }
}
