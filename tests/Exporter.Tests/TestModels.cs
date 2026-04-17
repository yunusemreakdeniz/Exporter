namespace Exporter.Tests;

public class Employee
{
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
}

public static class TestData
{
    public static List<Employee> CreateEmployees() =>
    [
        new()
        {
            Name = "Ahmet Yılmaz",
            Department = "Yazılım",
            Email = "ahmet@test.com",
            Salary = 25000m,
            BirthDate = new DateTime(1990, 5, 15),
            IsActive = true
        },
        new()
        {
            Name = "Ayşe Demir",
            Department = "Pazarlama",
            Email = "ayse@test.com",
            Salary = 22000m,
            BirthDate = new DateTime(1985, 8, 22),
            IsActive = true
        },
        new()
        {
            Name = "Mehmet Kaya",
            Department = "Yazılım",
            Email = "mehmet@test.com",
            Salary = 28000m,
            BirthDate = new DateTime(1992, 1, 10),
            IsActive = false
        }
    ];
}
