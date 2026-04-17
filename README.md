# Exporter

Lambda expression tabanlı, fluent API ile kullanılan, pluggable provider mimarisiyle birden fazla export formatını destekleyen bir .NET export kütüphanesi.

## Özellikler

- **Lambda Expression** ile kolon eşleme — `x => x.Name`
- **Fluent API** ile kolon konfigürasyonu — Header, Format, Width, Style, Validate, Transform
- **4 Export Formatı** — Excel (.xlsx), CSV, PDF, JSON
- **Multi-Sheet** desteği — Birden fazla sayfa tanımlama
- **Stil Desteği** — Bold, Italic, FontSize, FontColor, BackgroundColor, Alignment, Border
- **Validasyon** — Required, MaxLength, MinLength, Range, Regex, Custom kuralları
- **Transform** — Export öncesi veri dönüşümü
- **Pluggable Mimari** — `IExportProvider` ile özel format ekleme

## Kurulum

Projeye ihtiyacınız olan provider paketini referans olarak ekleyin:

```xml
<!-- Excel desteği için -->
<PackageReference Include="Exporter.Excel" />

<!-- CSV desteği için -->
<PackageReference Include="Exporter.Csv" />

<!-- PDF desteği için -->
<PackageReference Include="Exporter.Pdf" />

<!-- JSON desteği için -->
<PackageReference Include="Exporter.Json" />
```

## Hızlı Başlangıç

```csharp
using Exporter.Core;
using Exporter.Core.Models;
using Exporter.Excel;

var employees = GetEmployees();

Exporter.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col
        .Header("Ad Soyad")
        .Width(30)
        .Style(s => s.Bold().FontColor("#333")))
    .AddColumn(x => x.Salary, col => col
        .Header("Maaş")
        .Format("C2")
        .Style(s => s.Alignment(Alignment.Right)))
    .AddColumn(x => x.BirthDate, col => col
        .Header("Doğum Tarihi")
        .Format("dd.MM.yyyy"))
    .AddColumn(x => x.Email, col => col
        .Header("E-posta")
        .Validate(v => v.Required().MaxLength(100)))
    .ToExcel(opt =>
    {
        opt.AutoFilter = true;
        opt.FreezeTopRow = true;
    })
    .Export("rapor.xlsx");
```

## Kullanım Detayları

### Kolon Tanımlama

```csharp
.AddColumn(x => x.Name)                          // otomatik header: "Name"
.AddColumn(x => x.Name, col => col.Header("Ad")) // özel header
```

### Format

```csharp
.AddColumn(x => x.Salary, col => col.Format("C2"))         // para birimi
.AddColumn(x => x.BirthDate, col => col.Format("dd.MM.yyyy")) // tarih
.AddColumn(x => x.Rate, col => col.Format("P1"))           // yüzde
```

### Stil

```csharp
.AddColumn(x => x.Name, col => col
    .Style(s => s
        .Bold()
        .Italic()
        .FontSize(14)
        .FontColor("#FF0000")
        .BackgroundColor("#FFFF00")
        .Alignment(Alignment.Center)
        .Border(BorderStyle.Thin)))
```

### Header Stili

```csharp
.HeaderStyle(s => s.Bold().BackgroundColor("#4472C4").FontColor("#FFFFFF"))
```

### Validasyon

```csharp
.AddColumn(x => x.Email, col => col
    .Validate(v => v
        .Required()
        .MaxLength(100)
        .Regex(@"^[\w.-]+@[\w.-]+\.\w+$", "Geçerli bir e-posta adresi giriniz")))
```

Validasyonu ayrıca çalıştırabilirsiniz:

```csharp
var result = builder.ToExcel();
var validation = result.Validate();

if (!validation.IsValid)
{
    foreach (var error in validation.Errors)
        Console.WriteLine($"Satır {error.RowIndex}: {error.ColumnName} - {error.Message}");
}
```

### Transform

```csharp
.AddColumn(x => x.IsActive, col => col
    .Header("Durum")
    .Transform(v => v ? "Aktif" : "Pasif"))
```

### Multi-Sheet

```csharp
Exporter.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col.Header("Ad"))
    .AddColumn(x => x.Department, col => col.Header("Departman"))
    .Sheet("Maaş Detayı")
    .AddColumn(x => x.Name, col => col.Header("Ad"))
    .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("C2"))
    .ToExcel()
    .Export("rapor.xlsx");
```

### Export Seçenekleri

```csharp
// Dosyaya kaydet
result.Export("rapor.xlsx");

// byte[] olarak al
byte[] bytes = result.Export();

// Stream olarak al
using var stream = result.ExportAsStream();

// Stream'e yaz
result.Export(myStream);
```

### Provider'lar

#### Excel

```csharp
.ToExcel(opt =>
{
    opt.AutoFilter = true;
    opt.FreezeTopRow = true;
})
```

#### CSV

```csharp
.ToCsv(opt =>
{
    opt.Delimiter = ";";
    opt.IncludeHeader = true;
    opt.Encoding = Encoding.UTF8;
})
```

#### PDF

```csharp
.ToPdf(opt =>
{
    opt.Title = "Çalışan Raporu";
})
```

#### JSON

```csharp
.ToJson(opt =>
{
    opt.Indented = true;
})
```

## Proje Yapısı

```
Exporter/
├── src/
│   ├── Exporter.Core/       Core kütüphane (modeller, builder, validasyon)
│   ├── Exporter.Excel/      Excel provider (ClosedXML)
│   ├── Exporter.Csv/        CSV provider
│   ├── Exporter.Pdf/        PDF provider (QuestPDF)
│   └── Exporter.Json/       JSON provider (System.Text.Json)
├── tests/
│   └── Exporter.Tests/      Unit testler (47 test)
└── samples/
    └── Exporter.Sample/     Örnek konsol uygulaması
```

## Özel Provider Yazma

`IExportProvider` arayüzünü implemente ederek kendi formatınızı ekleyebilirsiniz:

```csharp
public class XmlExportProvider : IExportProvider
{
    public byte[] Export<T>(ExportConfiguration<T> config) where T : class
    {
        // XML oluşturma mantığı
    }

    public void Export<T>(ExportConfiguration<T> config, Stream stream) where T : class
    {
        // Stream'e yazma mantığı
    }
}

// Kullanım
var provider = new XmlExportProvider();
builder.To(provider).Export("rapor.xml");
```

## Lisans

MIT
