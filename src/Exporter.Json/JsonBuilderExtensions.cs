using Exporter.Core;
using Exporter.Core.Builders;

namespace Exporter.Json;

public static class JsonBuilderExtensions
{
    public static ExportResult<T> ToJson<T>(this ExportBuilder<T> builder, Action<JsonOptions>? options = null) where T : class
    {
        var provider = new JsonExportProvider();

        if (options != null)
        {
            var opts = new JsonOptions();
            options(opts);
            return builder.To(provider, dict =>
            {
                dict["Indented"] = opts.Indented;
            });
        }

        return builder.To(provider);
    }
}

public class JsonOptions
{
    public bool Indented { get; set; } = true;
}
