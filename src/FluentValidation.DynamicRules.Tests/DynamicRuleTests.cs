using FluentValidation.DynamicRules.Validators;
using Xunit;

namespace FluentValidation.DynamicRules.Tests;

public class DynamicRuleTests {
  [Fact]
  public void Parser_Parse_String_NotEmpty() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""firstName""><not-empty message=""value cannot be empty""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("value cannot be empty", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotEmptyValidator", result1.Errors[0].ErrorCode);
  }
  
  [Fact]
  public void Parser_Parse_String_NotEmptyWithInt() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""Age""><not-empty /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must not be empty.", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotEmptyValidator", result1.Errors[0].ErrorCode);
  }
  
  [Fact]
  public void Parser_Parse_String_NotEmptyWithIntWithCustomMessage() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""Age""><not-empty message=""value cannot be zero""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("value cannot be zero", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotEmptyValidator", result1.Errors[0].ErrorCode);
  }
  
  
  [Fact]
  public void Parser_Parse_String_MustBe() {
    var parser = new RuleParser();
    var xml =
      @"<rules><rule-for prop=""firstName""><must-be call=""MustBeAhmed"" message=""This is not a valid firstName"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("This is not a valid firstName", result1.Errors[0].ErrorMessage);
    Assert.Equal("PredicateValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_NotEqual() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""age""><not-equal value=""0""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 0;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must not be equal to '0'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotEqualValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LengthWithRange() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""firstName""><string-len min=""10"" max=""20"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.True(result1.IsValid);

    person1.FirstName = "t";
    result1 = validator.Validate(person1);

    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("'First Name' must be between 10 and 20 characters. You entered 1 characters.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("LengthValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LengthWithFixedValue() {
    var parser = new RuleParser();
    var xml = @"<rules><rule-for prop=""firstName""><string-len value=""5"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.True(result1.IsValid);

    person1.FirstName = "123456";
    result1 = validator.Validate(person1);

    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("'First Name' must be between 5 and 5 characters. You entered 6 characters.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("LengthValidator", result1.Errors[0].ErrorCode);
  }

  public class Person {
    public string FirstName { get; set; }
    public int Age { get; set; }
  }

  public class PersonValidator : AbstractDynamicValidator<Person> {
    public PersonValidator(ValidationBuilder builder) : base(builder) { }
    private bool MustBeAhmed(string value) => !string.IsNullOrEmpty(value) && value.ToLower() == "ahmed";
  }
}