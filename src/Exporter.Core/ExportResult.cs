using Exporter.Core.Abstractions;
using Exporter.Core.Models;
using Exporter.Core.Validation;

namespace Exporter.Core;

public class ExportResult<T> where T : class
{
    private readonly IExportProvider _provider;
    private readonly ExportConfiguration<T> _configuration;

    internal ExportResult(IExportProvider provider, ExportConfiguration<T> configuration)
    {
        _provider = provider;
        _configuration = configuration;
    }

    public byte[] Export()
    {
        return _provider.Export(_configuration);
    }

    public void Export(string filePath)
    {
        var bytes = _provider.Export(_configuration);
        File.WriteAllBytes(filePath, bytes);
    }

    public void Export(Stream stream)
    {
        _provider.Export(_configuration, stream);
    }

    public MemoryStream ExportAsStream()
    {
        var bytes = _provider.Export(_configuration);
        return new MemoryStream(bytes);
    }

    public ValidationResult Validate()
    {
        return ValidationEngine.Validate(_configuration);
    }
}
