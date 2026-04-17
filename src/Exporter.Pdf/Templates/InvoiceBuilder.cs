namespace Exporter.Pdf.Templates;

public class InvoiceBuilder
{
    private readonly InvoiceData _data = new();

    private InvoiceBuilder() { }

    public static InvoiceBuilder Create() => new();

    public InvoiceBuilder Company(Action<CompanyInfoBuilder> configure)
    {
        var builder = new CompanyInfoBuilder(_data.Company);
        configure(builder);
        return this;
    }

    public InvoiceBuilder Customer(Action<CustomerInfoBuilder> configure)
    {
        var builder = new CustomerInfoBuilder(_data.Customer);
        configure(builder);
        return this;
    }

    public InvoiceBuilder Info(Action<InvoiceInfoBuilder> configure)
    {
        var builder = new InvoiceInfoBuilder(_data.Info);
        configure(builder);
        return this;
    }

    public InvoiceBuilder AddItem(string description, decimal quantity, decimal unitPrice, decimal taxRate = 0)
    {
        _data.Items.Add(new InvoiceItem
        {
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRate = taxRate
        });
        return this;
    }

    public InvoiceBuilder Notes(string notes)
    {
        _data.Notes = notes;
        return this;
    }

    public InvoiceBuilder BankInfo(Action<BankDetailsBuilder> configure)
    {
        _data.Bank ??= new BankDetails();
        var builder = new BankDetailsBuilder(_data.Bank);
        configure(builder);
        return this;
    }

    public InvoiceBuilder Theme(PdfColorTheme theme)
    {
        _data.Theme = theme;
        return this;
    }

    public void Build(string filePath)
    {
        var bytes = InvoiceRenderer.Render(_data);
        File.WriteAllBytes(filePath, bytes);
    }

    public byte[] Build()
    {
        return InvoiceRenderer.Render(_data);
    }

    public void Build(Stream stream)
    {
        InvoiceRenderer.Render(_data, stream);
    }
}

// --- Sub-builders ---

public class CompanyInfoBuilder(CompanyInfo info)
{
    public CompanyInfoBuilder Name(string name) { info.Name = name; return this; }
    public CompanyInfoBuilder Address(string address) { info.Address = address; return this; }
    public CompanyInfoBuilder Phone(string phone) { info.Phone = phone; return this; }
    public CompanyInfoBuilder Email(string email) { info.Email = email; return this; }
    public CompanyInfoBuilder TaxOffice(string taxOffice) { info.TaxOffice = taxOffice; return this; }
    public CompanyInfoBuilder TaxNumber(string taxNumber) { info.TaxNumber = taxNumber; return this; }
    public CompanyInfoBuilder Logo(string logoPath) { info.LogoPath = logoPath; return this; }
}

public class CustomerInfoBuilder(CustomerInfo info)
{
    public CustomerInfoBuilder Name(string name) { info.Name = name; return this; }
    public CustomerInfoBuilder Address(string address) { info.Address = address; return this; }
    public CustomerInfoBuilder Phone(string phone) { info.Phone = phone; return this; }
    public CustomerInfoBuilder Email(string email) { info.Email = email; return this; }
    public CustomerInfoBuilder TaxOffice(string taxOffice) { info.TaxOffice = taxOffice; return this; }
    public CustomerInfoBuilder TaxNumber(string taxNumber) { info.TaxNumber = taxNumber; return this; }
}

public class InvoiceInfoBuilder(InvoiceInfo info)
{
    public InvoiceInfoBuilder InvoiceNumber(string number) { info.InvoiceNumber = number; return this; }
    public InvoiceInfoBuilder Date(DateTime date) { info.Date = date; return this; }
    public InvoiceInfoBuilder DueDate(DateTime dueDate) { info.DueDate = dueDate; return this; }
    public InvoiceInfoBuilder Currency(string currency) { info.Currency = currency; return this; }
}

public class BankDetailsBuilder(BankDetails info)
{
    public BankDetailsBuilder BankName(string name) { info.BankName = name; return this; }
    public BankDetailsBuilder Iban(string iban) { info.Iban = iban; return this; }
    public BankDetailsBuilder AccountHolder(string holder) { info.AccountHolder = holder; return this; }
}
