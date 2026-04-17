using Exporter.Core;
using Exporter.Core.Builders;

namespace Exporter.Excel;

public static class ExcelBuilderExtensions
{
    public static ExportResult<T> ToExcel<T>(this ExportBuilder<T> builder, Action<ExcelOptions>? options = null) where T : class
    {
        var provider = new ExcelExportProvider();

        if (options != null)
        {
            var opts = new ExcelOptions();
            options(opts);
            return builder.To(provider, dict =>
            {
                if (opts.AutoFilter) dict["AutoFilter"] = true;
                if (opts.FreezeTopRow) dict["FreezeTopRow"] = true;
            });
        }

        return builder.To(provider);
    }
}

public class ExcelOptions
{
    public bool AutoFilter { get; set; }
    public bool FreezeTopRow { get; set; }
}
