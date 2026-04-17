using System.Text;
using Exporter.Core.Abstractions;
using Exporter.Core.Models;

namespace Exporter.Csv;

public class CsvExportProvider : IExportProvider
{
    public byte[] Export<T>(ExportConfiguration<T> configuration) where T : class
    {
        using var stream = new MemoryStream();
        Export(configuration, stream);
        return stream.ToArray();
    }

    public void Export<T>(ExportConfiguration<T> configuration, Stream stream) where T : class
    {
        var delimiter = GetOption<T, string>(configuration, "Delimiter") ?? ",";
        var encoding = GetOption<T, Encoding>(configuration, "Encoding") ?? Encoding.UTF8;
        var includeHeader = GetOption<T, bool?>(configuration, "IncludeHeader") ?? true;

        using var writer = new StreamWriter(stream, encoding, leaveOpen: true);

        foreach (var sheet in configuration.Sheets)
        {
            if (configuration.Sheets.Count > 1)
                writer.WriteLine($"--- {sheet.Name} ---");

            if (includeHeader)
            {
                var headers = sheet.Columns.Select(c => EscapeCsvField(c.Header, delimiter));
                writer.WriteLine(string.Join(delimiter, headers));
            }

            foreach (var item in configuration.Data)
            {
                var values = sheet.Columns.Select(c => EscapeCsvField(c.GetFormattedValue(item), delimiter));
                writer.WriteLine(string.Join(delimiter, values));
            }
        }

        writer.Flush();
    }

    private static string EscapeCsvField(string field, string delimiter)
    {
        if (field.Contains(delimiter) || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            return $"\"{field.Replace("\"", "\"\"")}\"";
        return field;
    }

    private static TOption? GetOption<T, TOption>(ExportConfiguration<T> configuration, string key) where T : class
    {
        if (configuration.ProviderOptions.TryGetValue(key, out var value) && value is TOption typed)
            return typed;
        return default;
    }
}
