using Exporter.Core.Builders;
using Exporter.Core.Validation;
using Exporter.Core.Validation.Rules;

namespace Exporter.Tests;

public class ValidationTests
{
    [Fact]
    public void RequiredRule_NullValue_Fails()
    {
        var rule = new RequiredRule();
        Assert.False(rule.IsValid(null));
    }

    [Fact]
    public void RequiredRule_EmptyString_Fails()
    {
        var rule = new RequiredRule();
        Assert.False(rule.IsValid(""));
        Assert.False(rule.IsValid("   "));
    }

    [Fact]
    public void RequiredRule_ValidValue_Passes()
    {
        var rule = new RequiredRule();
        Assert.True(rule.IsValid("hello"));
        Assert.True(rule.IsValid(42));
    }

    [Fact]
    public void MaxLengthRule_ExceedsMax_Fails()
    {
        var rule = new MaxLengthRule(5);
        Assert.False(rule.IsValid("toolong"));
    }

    [Fact]
    public void MaxLengthRule_WithinMax_Passes()
    {
        var rule = new MaxLengthRule(10);
        Assert.True(rule.IsValid("hello"));
        Assert.True(rule.IsValid(null));
    }

    [Fact]
    public void MinLengthRule_BelowMin_Fails()
    {
        var rule = new MinLengthRule(5);
        Assert.False(rule.IsValid("hi"));
        Assert.False(rule.IsValid(null));
    }

    [Fact]
    public void MinLengthRule_MeetsMin_Passes()
    {
        var rule = new MinLengthRule(3);
        Assert.True(rule.IsValid("hello"));
    }

    [Fact]
    public void RangeRule_OutOfRange_Fails()
    {
        var rule = new RangeRule(10, 100);
        Assert.False(rule.IsValid(5));
        Assert.False(rule.IsValid(150));
        Assert.False(rule.IsValid(null));
    }

    [Fact]
    public void RangeRule_InRange_Passes()
    {
        var rule = new RangeRule(10, 100);
        Assert.True(rule.IsValid(10));
        Assert.True(rule.IsValid(50));
        Assert.True(rule.IsValid(100));
    }

    [Fact]
    public void RegexRule_NoMatch_Fails()
    {
        var rule = new RegexRule(@"^\d+$");
        Assert.False(rule.IsValid("abc"));
        Assert.False(rule.IsValid(null));
    }

    [Fact]
    public void RegexRule_Matches_Passes()
    {
        var rule = new RegexRule(@"^\d+$");
        Assert.True(rule.IsValid("123"));
    }

    [Fact]
    public void CustomRule_PredicateFails_Fails()
    {
        var rule = new CustomRule(v => v is string s && s.Contains('@'), "Must contain @");
        Assert.False(rule.IsValid("noemail"));
    }

    [Fact]
    public void CustomRule_PredicatePasses_Passes()
    {
        var rule = new CustomRule(v => v is string s && s.Contains('@'), "Must contain @");
        Assert.True(rule.IsValid("test@email.com"));
    }

    [Fact]
    public void ValidationEngine_Validate_ReturnsErrors()
    {
        var employees = new List<Employee>
        {
            new() { Name = "", Email = "invalid", Salary = 1000m, Department = "IT", BirthDate = DateTime.Now },
            new() { Name = "Test", Email = "valid@test.com", Salary = 2000m, Department = "HR", BirthDate = DateTime.Now }
        };

        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Validate(v => v.Required()))
            .AddColumn(x => x.Email, col => col.Validate(v => v
                .Required()
                .Custom(val => val is string s && s.Contains('@'), "Must be a valid email")))
            .BuildConfiguration();

        var result = ValidationEngine.Validate(config);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
    }

    [Fact]
    public void ValidationEngine_ValidData_NoErrors()
    {
        var employees = TestData.CreateEmployees();

        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Validate(v => v.Required()))
            .AddColumn(x => x.Email, col => col.Validate(v => v.Required()))
            .BuildConfiguration();

        var result = ValidationEngine.Validate(config);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidationBuilder_ChainsRules()
    {
        var employees = new List<Employee>
        {
            new() { Name = "A", Email = "x", Salary = 1000m, Department = "IT", BirthDate = DateTime.Now }
        };

        var config = ExportBuilder<Employee>.Create(employees)
            .AddColumn(x => x.Name, col => col.Validate(v => v.Required().MinLength(3).MaxLength(50)))
            .BuildConfiguration();

        var result = ValidationEngine.Validate(config);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("at least 3", result.Errors[0].Message);
    }
}
