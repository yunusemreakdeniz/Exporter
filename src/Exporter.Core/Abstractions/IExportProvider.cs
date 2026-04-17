using Exporter.Core.Models;

namespace Exporter.Core.Abstractions;

public interface IExportProvider
{
    byte[] Export<T>(ExportConfiguration<T> configuration) where T : class;
    void Export<T>(ExportConfiguration<T> configuration, Stream stream) where T : class;
}
