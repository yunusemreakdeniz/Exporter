using Exporter.Core;
using Exporter.Core.Builders;

namespace Exporter.Csv;

public static class CsvBuilderExtensions
{
    public static ExportResult<T> ToCsv<T>(this ExportBuilder<T> builder, Action<CsvOptions>? options = null) where T : class
    {
        var provider = new CsvExportProvider();

        if (options != null)
        {
            var opts = new CsvOptions();
            options(opts);
            return builder.To(provider, dict =>
            {
                dict["Delimiter"] = opts.Delimiter;
                dict["IncludeHeader"] = opts.IncludeHeader;
                if (opts.Encoding != null) dict["Encoding"] = opts.Encoding;
            });
        }

        return builder.To(provider);
    }
}

public class CsvOptions
{
    public string Delimiter { get; set; } = ",";
    public bool IncludeHeader { get; set; } = true;
    public System.Text.Encoding? Encoding { get; set; }
}
