namespace Exporter.Pdf.Templates;

public class InvoiceData
{
    public CompanyInfo Company { get; set; } = new();
    public CustomerInfo Customer { get; set; } = new();
    public InvoiceInfo Info { get; set; } = new();
    public List<InvoiceItem> Items { get; set; } = [];
    public string? Notes { get; set; }
    public BankDetails? Bank { get; set; }
    public PdfColorTheme Theme { get; set; } = PdfThemes.Blue;

    public decimal Subtotal => Items.Sum(i => i.LineTotal);
    public decimal TotalTax => Items.Sum(i => i.TaxAmount);
    public decimal GrandTotal => Subtotal + TotalTax;
}

public class CompanyInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public string? LogoPath { get; set; }
}

public class CustomerInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
}

public class InvoiceInfo
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public string? Currency { get; set; } = "₺";
}

public class InvoiceItem
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;
    public decimal TaxAmount => LineTotal * TaxRate / 100m;
    public decimal GrossTotal => LineTotal + TaxAmount;
}

public class BankDetails
{
    public string BankName { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string? AccountHolder { get; set; }
}
