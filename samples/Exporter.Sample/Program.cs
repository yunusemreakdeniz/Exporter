using Exporter.Core.Builders;
using Exporter.Core.Models;
using Exporter.Excel;
using Exporter.Csv;
using Exporter.Pdf;
using Exporter.Pdf.Templates;
using Exporter.Json;

using Export = Exporter.Core.Exporter;

// --- Örnek Veri ---
var employees = new List<Employee>
{
    new("Ahmet Yılmaz",   "Yazılım",    "ahmet@firma.com",   32000m, new(1990, 5, 15), true),
    new("Ayşe Demir",     "Pazarlama",  "ayse@firma.com",    28000m, new(1985, 8, 22), true),
    new("Mehmet Kaya",    "Yazılım",    "mehmet@firma.com",  35000m, new(1992, 1, 10), false),
    new("Fatma Çelik",    "İK",         "fatma@firma.com",   26000m, new(1988, 11, 3), true),
    new("Ali Öztürk",     "Finans",     "ali@firma.com",     30000m, new(1995, 7, 20), true),
    new("Zeynep Arslan",  "Yazılım",    "zeynep@firma.com",  33000m, new(1993, 3, 8),  true),
    new("Emre Yıldız",    "Pazarlama",  "",                  24000m, new(1998, 12, 1), false),
};

var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
Directory.CreateDirectory(outputDir);

Console.WriteLine("=== Exporter Kütüphanesi - Örnek Uygulama ===\n");

// ───────────────────────────────────────────────
// 1) EXCEL EXPORT
// ───────────────────────────────────────────────
Console.WriteLine("1) Excel Export");

var excelPath = Path.Combine(outputDir, "calisanlar.xlsx");

Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col
        .Header("Ad Soyad")
        .Width(25)
        .Style(s => s.Bold()))
    .AddColumn(x => x.Department, col => col
        .Header("Departman")
        .Width(15))
    .AddColumn(x => x.Salary, col => col
        .Header("Maaş (₺)")
        .Format("N0")
        .Style(s => s.Alignment(Alignment.Right)))
    .AddColumn(x => x.BirthDate, col => col
        .Header("Doğum Tarihi")
        .Format("dd.MM.yyyy"))
    .AddColumn(x => x.IsActive, col => col
        .Header("Durum")
        .Transform(v => v ? "Aktif" : "Pasif"))
    .HeaderStyle(s => s.Bold().BackgroundColor("#2F5496").FontColor("#FFFFFF"))
    .Sheet("Departman Bazlı")
    .AddColumn(x => x.Department, col => col.Header("Departman").Width(15))
    .AddColumn(x => x.Name, col => col.Header("Çalışan").Width(25))
    .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("N0"))
    .HeaderStyle(s => s.Bold().BackgroundColor("#548235").FontColor("#FFFFFF"))
    .ToExcel(opt =>
    {
        opt.AutoFilter = true;
        opt.FreezeTopRow = true;
    })
    .Export(excelPath);

Console.WriteLine($"   -> {excelPath}");

// ───────────────────────────────────────────────
// 2) CSV EXPORT
// ───────────────────────────────────────────────
Console.WriteLine("\n2) CSV Export");

var csvPath = Path.Combine(outputDir, "calisanlar.csv");

var csvResult = Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col.Header("Ad Soyad"))
    .AddColumn(x => x.Department, col => col.Header("Departman"))
    .AddColumn(x => x.Email, col => col.Header("E-posta"))
    .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("N0"))
    .AddColumn(x => x.IsActive, col => col
        .Header("Durum")
        .Transform(v => v ? "Aktif" : "Pasif"))
    .ToCsv(opt => opt.Delimiter = ";");

csvResult.Export(csvPath);

Console.WriteLine($"   -> {csvPath}");
Console.WriteLine("\n   CSV İçeriği:");
var csvContent = System.Text.Encoding.UTF8.GetString(csvResult.Export());
foreach (var line in csvContent.Split('\n', StringSplitOptions.RemoveEmptyEntries))
    Console.WriteLine($"   {line.TrimEnd()}");

// ───────────────────────────────────────────────
// 3) JSON EXPORT
// ───────────────────────────────────────────────
Console.WriteLine("\n3) JSON Export");

var jsonPath = Path.Combine(outputDir, "calisanlar.json");

var jsonResult = Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col.Header("ad"))
    .AddColumn(x => x.Department, col => col.Header("departman"))
    .AddColumn(x => x.Email, col => col.Header("email"))
    .AddColumn(x => x.Salary, col => col.Header("maas"))
    .AddColumn(x => x.IsActive, col => col.Header("aktif"))
    .ToJson();

jsonResult.Export(jsonPath);

Console.WriteLine($"   -> {jsonPath}");
Console.WriteLine("\n   JSON İçeriği:");
var jsonContent = System.Text.Encoding.UTF8.GetString(jsonResult.Export());
Console.WriteLine(jsonContent);

// ───────────────────────────────────────────────
// 4) PDF EXPORT — Profesyonel (Blue tema, kapak, filigran)
// ───────────────────────────────────────────────
Console.WriteLine("4) PDF Export — Blue Tema (Kapak + Filigran)");

var pdfPath = Path.Combine(outputDir, "calisanlar.pdf");

Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col
        .Header("Ad Soyad")
        .Width(120)
        .Style(s => s.Bold()))
    .AddColumn(x => x.Department, col => col.Header("Departman"))
    .AddColumn(x => x.Email, col => col.Header("E-posta"))
    .AddColumn(x => x.Salary, col => col
        .Header("Maaş (₺)")
        .Format("N0")
        .Style(s => s.Alignment(Alignment.Right)))
    .AddColumn(x => x.BirthDate, col => col
        .Header("Doğum Tarihi")
        .Format("dd.MM.yyyy")
        .Style(s => s.Alignment(Alignment.Center)))
    .AddColumn(x => x.IsActive, col => col
        .Header("Durum")
        .Transform(v => v ? "Aktif" : "Pasif")
        .Style(s => s.Alignment(Alignment.Center)))
    .ToPdf(opt =>
    {
        opt.Title = "Çalışan Listesi";
        opt.Subtitle = "Nisan 2026 — İnsan Kaynakları Raporu";
        opt.CompanyName = "Acme Teknoloji A.Ş.";
        opt.Theme = PdfThemes.Blue;
        opt.ShowCoverPage = true;
        opt.ShowSummary = true;
        opt.ZebraRows = true;
        opt.Watermark = "GİZLİ";
        opt.FooterNote = "Bu rapor şirket içi kullanım amaçlıdır.";
    })
    .Export(pdfPath);

Console.WriteLine($"   -> {pdfPath}");

// ───────────────────────────────────────────────
// 4b) PDF EXPORT — Green tema, landscape, kapaksız
// ───────────────────────────────────────────────
Console.WriteLine("\n4b) PDF Export — Green Tema (Landscape)");

var pdfPath2 = Path.Combine(outputDir, "calisanlar_green.pdf");

Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col.Header("Ad Soyad").Width(130).Style(s => s.Bold()))
    .AddColumn(x => x.Department, col => col.Header("Departman"))
    .AddColumn(x => x.Email, col => col.Header("E-posta"))
    .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("N0").Style(s => s.Alignment(Alignment.Right)))
    .AddColumn(x => x.BirthDate, col => col.Header("Doğum Tarihi").Format("dd.MM.yyyy"))
    .AddColumn(x => x.IsActive, col => col.Header("Durum").Transform(v => v ? "Aktif" : "Pasif"))
    .ToPdf(opt =>
    {
        opt.Title = "Personel Durum Raporu";
        opt.CompanyName = "Acme Teknoloji A.Ş.";
        opt.Theme = PdfThemes.Green;
        opt.PageOrientation = PageOrientation.Landscape;
        opt.ShowCoverPage = false;
        opt.ZebraRows = true;
    })
    .Export(pdfPath2);

Console.WriteLine($"   -> {pdfPath2}");

// ───────────────────────────────────────────────
// 4c) PDF EXPORT — Dark tema
// ───────────────────────────────────────────────
Console.WriteLine("\n4c) PDF Export — Dark Tema");

var pdfPath3 = Path.Combine(outputDir, "calisanlar_dark.pdf");

Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col.Header("Ad Soyad").Style(s => s.Bold()))
    .AddColumn(x => x.Department, col => col.Header("Departman"))
    .AddColumn(x => x.Salary, col => col.Header("Maaş").Format("N0").Style(s => s.Alignment(Alignment.Right)))
    .AddColumn(x => x.IsActive, col => col.Header("Durum").Transform(v => v ? "Aktif" : "Pasif"))
    .ToPdf(opt =>
    {
        opt.Title = "Çalışan Özet";
        opt.Subtitle = "Yönetim Kurulu Sunumu";
        opt.CompanyName = "Acme Teknoloji A.Ş.";
        opt.Theme = PdfThemes.Dark;
        opt.ShowCoverPage = true;
        opt.Watermark = "TASLAK";
    })
    .Export(pdfPath3);

Console.WriteLine($"   -> {pdfPath3}");

// ───────────────────────────────────────────────
// 5) VALIDASYON ÖRNEĞİ
// ───────────────────────────────────────────────
Console.WriteLine("\n5) Validasyon Örneği");

var validationResult = Export.Create<Employee>(employees)
    .AddColumn(x => x.Name, col => col
        .Header("Ad Soyad")
        .Validate(v => v.Required().MinLength(3)))
    .AddColumn(x => x.Email, col => col
        .Header("E-posta")
        .Validate(v => v.Required().Regex(@"^[\w.-]+@[\w.-]+\.\w+$", "Geçerli bir e-posta giriniz")))
    .AddColumn(x => x.Salary, col => col
        .Header("Maaş")
        .Validate(v => v.Custom(val => val is decimal d && d >= 25000, "Maaş en az 25.000 ₺ olmalı")))
    .ToExcel()
    .Validate();

if (!validationResult.IsValid)
{
    Console.WriteLine($"   {validationResult.Errors.Count} validasyon hatası bulundu:\n");
    foreach (var error in validationResult.Errors)
    {
        var employee = employees[error.RowIndex];
        Console.WriteLine($"   [{error.RowIndex}] {employee.Name,-18} | {error.ColumnName,-10} | {error.Message}");
    }
}
else
{
    Console.WriteLine("   Tüm veriler geçerli!");
}

// ───────────────────────────────────────────────
// 6) FATURA PDF — InvoiceBuilder
// ───────────────────────────────────────────────
Console.WriteLine("\n6) Fatura PDF — Blue Tema");

var invoicePath = Path.Combine(outputDir, "fatura.pdf");

InvoiceBuilder.Create()
    .Company(c => c
        .Name("Acme Teknoloji A.Ş.")
        .Address("Levent Mah. İş Kuleleri No:42, Beşiktaş / İstanbul")
        .Phone("+90 212 555 00 00")
        .Email("info@acmetech.com.tr")
        .TaxOffice("Beşiktaş")
        .TaxNumber("1234567890"))
    .Customer(c => c
        .Name("Yıldız Yazılım Ltd. Şti.")
        .Address("Bağdat Cad. No:128, Kadıköy / İstanbul")
        .Phone("+90 216 444 00 00")
        .Email("muhasebe@yildizyazilim.com")
        .TaxOffice("Kadıköy")
        .TaxNumber("9876543210"))
    .Info(i => i
        .InvoiceNumber("FTR-2026-0042")
        .Date(new DateTime(2026, 4, 17))
        .DueDate(new DateTime(2026, 5, 17))
        .Currency("₺"))
    .AddItem("Web Uygulama Geliştirme (React + .NET)", 1, 45000m, 20)
    .AddItem("Mobil Uygulama Geliştirme (Flutter)",     1, 35000m, 20)
    .AddItem("Sunucu Kurulumu ve Konfigürasyon",        2, 8000m,  20)
    .AddItem("SSL Sertifikası (Yıllık)",                3, 1200m,  20)
    .AddItem("Aylık Bakım ve Destek Paketi",            6, 5000m,  20)
    .AddItem("UI/UX Tasarım Danışmanlığı",              10, 2500m, 20)
    .Notes("Ödeme, fatura tarihinden itibaren 30 gün içinde yukarıda belirtilen banka hesabına yapılmalıdır.\nGecikmeli ödemelerde aylık %2 gecikme faizi uygulanır.")
    .BankInfo(b => b
        .BankName("Garanti BBVA — Levent Şubesi")
        .Iban("TR12 0006 2000 0000 0012 3456 78")
        .AccountHolder("Acme Teknoloji A.Ş."))
    .Theme(PdfThemes.Blue)
    .Build(invoicePath);

Console.WriteLine($"   -> {invoicePath}");

// ───────────────────────────────────────────────
// 6b) FATURA PDF — Dark Tema
// ───────────────────────────────────────────────
Console.WriteLine("\n6b) Fatura PDF — Dark Tema");

var invoicePath2 = Path.Combine(outputDir, "fatura_dark.pdf");

InvoiceBuilder.Create()
    .Company(c => c
        .Name("Acme Teknoloji A.Ş.")
        .Address("Levent Mah. İş Kuleleri No:42, İstanbul")
        .Phone("+90 212 555 00 00")
        .TaxOffice("Beşiktaş")
        .TaxNumber("1234567890"))
    .Customer(c => c
        .Name("Global Lojistik A.Ş.")
        .Address("Atatürk Organize Sanayi, Ankara")
        .TaxOffice("Sincan")
        .TaxNumber("5551234567"))
    .Info(i => i
        .InvoiceNumber("FTR-2026-0043")
        .Date(new DateTime(2026, 4, 17))
        .DueDate(new DateTime(2026, 6, 17)))
    .AddItem("ERP Yazılım Lisansı", 1, 120000m, 20)
    .AddItem("Kurulum ve Eğitim",   1, 25000m,  20)
    .AddItem("1 Yıllık Destek",     1, 36000m,  20)
    .Notes("60 gün vadeli faturadır.")
    .BankInfo(b => b
        .BankName("İş Bankası — Levent Ticari Şube")
        .Iban("TR98 0006 4000 0011 2345 6789 00")
        .AccountHolder("Acme Teknoloji A.Ş."))
    .Theme(PdfThemes.Dark)
    .Build(invoicePath2);

Console.WriteLine($"   -> {invoicePath2}");

Console.WriteLine($"\n=== Tüm dosyalar '{outputDir}' klasörüne kaydedildi ===");

// --- Model ---
public record Employee(
    string Name,
    string Department,
    string Email,
    decimal Salary,
    DateTime BirthDate,
    bool IsActive);
