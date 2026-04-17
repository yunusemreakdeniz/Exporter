using System.Text.Json;
using Exporter.Core.Abstractions;
using Exporter.Core.Models;

namespace Exporter.Json;

public class JsonExportProvider : IExportProvider
{
    public byte[] Export<T>(ExportConfiguration<T> configuration) where T : class
    {
        using var stream = new MemoryStream();
        Export(configuration, stream);
        return stream.ToArray();
    }

    public void Export<T>(ExportConfiguration<T> configuration, Stream stream) where T : class
    {
        var indented = GetOption<T, bool?>(configuration, "Indented") ?? true;
        var options = new JsonWriterOptions { Indented = indented };

        using var writer = new Utf8JsonWriter(stream, options);
        var data = configuration.Data.ToList();

        if (configuration.Sheets.Count == 1)
        {
            WriteSheetArray(writer, configuration.Sheets[0], data);
        }
        else
        {
            writer.WriteStartObject();
            foreach (var sheet in configuration.Sheets)
            {
                writer.WritePropertyName(sheet.Name);
                WriteSheetArray(writer, sheet, data);
            }
            writer.WriteEndObject();
        }

        writer.Flush();
    }

    private static void WriteSheetArray<T>(Utf8JsonWriter writer, SheetDefinition<T> sheet, List<T> data) where T : class
    {
        writer.WriteStartArray();

        foreach (var item in data)
        {
            writer.WriteStartObject();
            foreach (var column in sheet.Columns)
            {
                writer.WritePropertyName(column.Header);
                var value = column.GetValue(item);
                WriteJsonValue(writer, value);
            }
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }

    private static void WriteJsonValue(Utf8JsonWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.WriteNullValue();
                break;
            case string s:
                writer.WriteStringValue(s);
                break;
            case int i:
                writer.WriteNumberValue(i);
                break;
            case long l:
                writer.WriteNumberValue(l);
                break;
            case double d:
                writer.WriteNumberValue(d);
                break;
            case float f:
                writer.WriteNumberValue(f);
                break;
            case decimal dec:
                writer.WriteNumberValue(dec);
                break;
            case bool b:
                writer.WriteBooleanValue(b);
                break;
            case DateTime dt:
                writer.WriteStringValue(dt.ToString("O"));
                break;
            case DateTimeOffset dto:
                writer.WriteStringValue(dto.ToString("O"));
                break;
            default:
                writer.WriteStringValue(value.ToString());
                break;
        }
    }

    private static TOption? GetOption<T, TOption>(ExportConfiguration<T> configuration, string key) where T : class
    {
        if (configuration.ProviderOptions.TryGetValue(key, out var value) && value is TOption typed)
            return typed;
        return default;
    }
}
