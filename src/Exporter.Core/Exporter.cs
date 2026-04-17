using Exporter.Core.Builders;

namespace Exporter.Core;

public static class Exporter
{
    public static ExportBuilder<T> Create<T>(IEnumerable<T> data) where T : class
        => ExportBuilder<T>.Create(data);
}
