using FluentValidation.DynamicRules.Validators;
using Xunit;

namespace FluentValidation.DynamicRules.Tests;

public class DynamicRuleTests {
  [Fact]
  public void Parser_Parse_String_NotNull() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><not-null message=""value cannot be null""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = null;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("value cannot be null", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotNullValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_Null() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><null message=""value must be null""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("value must be null", result1.Errors[0].ErrorMessage);
    Assert.Equal("NullValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_Empty() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><empty message=""value must be empty""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("value must be empty", result1.Errors[0].ErrorMessage);
    Assert.Equal("EmptyValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_NotEmpty() {
    var parser = new RuleParser<Person>();
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
  public void Parser_Parse_Email() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""email""><email /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Email = "test";

    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Email), result1.Errors[0].PropertyName);
    Assert.Equal("'Email' is not a valid email address.", result1.Errors[0].ErrorMessage);
    Assert.Equal("EmailValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_CreditCard() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""creditCard""><credit-card /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.CreditCard = "1234";

    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.CreditCard), result1.Errors[0].PropertyName);
    Assert.Equal("'Credit Card' is not a valid credit card number.", result1.Errors[0].ErrorMessage);
    Assert.Equal("CreditCardValidator", result1.Errors[0].ErrorCode);
  }


  [Fact]
  public void Parser_Parse_String_NotEmptyWithInt() {
    var parser = new RuleParser<Person>();
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
    var parser = new RuleParser<Person>();
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
    var parser = new RuleParser<Person>();
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
  public void Parser_Parse_FirstName_MustBeWithParentObject() {
    var parser = new RuleParser<Person>();
    var xml =
      @"<rules><rule-for prop=""firstName""><must-be with-parent=""CustomWithParentObject"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("The specified condition was not met for 'First Name'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("PredicateValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_FirstName_MustBeWithParentObjectAndContext() {
    var parser = new RuleParser<Person>();
    var xml =
      @"<rules><rule-for prop=""firstName""><must-be with-context=""CustomWithContextAndParentObject"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("He is not Ahmed", result1.Errors[0].ErrorMessage);
    
  }

  [Fact]
  public void Parser_Parse_String_NotEqual() {
    var parser = new RuleParser<Person>();
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
  public void Parser_Parse_String_Equal() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><equal value=""0""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 10;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be equal to '0'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("EqualValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_NotEqualAnotherProp() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><not-equal prop=""lastName""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.FirstName = "Ali";
    person1.LastName = "Ali";
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("'First Name' must not be equal to 'Ali'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("NotEqualValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LengthWithRange() {
    var parser = new RuleParser<Person>();
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
  public void Parser_Parse_String_ExclusiveBetween() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><exclusive-between min=""10"" max=""20"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 15;
    var result1 = validator.Validate(person1);
    Assert.True(result1.IsValid);

    person1.Age = 20;
    result1 = validator.Validate(person1);

    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be between 10 and 20 (exclusive). You entered 20.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("ExclusiveBetweenValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_InclusiveBetween() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><inclusive-between min=""10"" max=""20"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 20;
    var result1 = validator.Validate(person1);
    Assert.True(result1.IsValid);

    person1.Age = 21;
    result1 = validator.Validate(person1);

    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be between 10 and 20. You entered 21.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("InclusiveBetweenValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LessThan() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><less-than value=""100""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 100;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be less than '100'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("LessThanValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LessThanOrEqual() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><less-than-equal value=""100""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 101;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be less than or equal to '100'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("LessThanOrEqualValidator", result1.Errors[0].ErrorCode);
  }[Fact]
  public void Parser_Parse_String_GreaterThan() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><greater-than value=""100""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 100;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be greater than '100'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("GreaterThanValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_GreaterThanOrEqual() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><greater-than-equal value=""100""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.Age = 99;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be greater than or equal to '100'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("GreaterThanOrEqualValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_LessThanOrEqualWithAnotherProperty() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""age""><less-than-equal prop=""anotherAge""/></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    person1.AnotherAge = 100;
    person1.Age = 101;
    var result1 = validator.Validate(person1);
    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.Age), result1.Errors[0].PropertyName);
    Assert.Equal("'Age' must be less than or equal to '100'.", result1.Errors[0].ErrorMessage);
    Assert.Equal("LessThanOrEqualValidator", result1.Errors[0].ErrorCode);
  }


  [Fact]
  public void Parser_Parse_String_LengthWithFixedValue() {
    var parser = new RuleParser<Person>();
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


  [Fact]
  public void Parser_Parse_String_MaxLength() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><max-len value=""5"" /></rule-for></rules>";
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
    Assert.Equal("The length of 'First Name' must be 5 characters or fewer. You entered 6 characters.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("MaximumLengthValidator", result1.Errors[0].ErrorCode);
  }

  [Fact]
  public void Parser_Parse_String_MinLength() {
    var parser = new RuleParser<Person>();
    var xml = @"<rules><rule-for prop=""firstName""><min-len value=""5"" /></rule-for></rules>";
    var builder = parser.Parse(xml);
    var validator = new PersonValidator(builder);

    var person1 = new Person();
    var result1 = validator.Validate(person1);
    Assert.True(result1.IsValid);

    person1.FirstName = "1234";
    result1 = validator.Validate(person1);

    Assert.False(result1.IsValid);
    Assert.NotEmpty(result1.Errors);
    Assert.Equal(nameof(Person.FirstName), result1.Errors[0].PropertyName);
    Assert.Equal("The length of 'First Name' must be at least 5 characters. You entered 4 characters.",
      result1.Errors[0].ErrorMessage);
    Assert.Equal("MinimumLengthValidator", result1.Errors[0].ErrorCode);
  }

  public class Person {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public int AnotherAge { get; set; }
    public string CreditCard { get; set; }
    public string Email { get; set; }
  }

  public class PersonValidator : AbstractDynamicValidator<Person> {
    public PersonValidator(ValidationBuilder<Person> builder) : base(builder) {
      //
    }

    private bool MustBeAhmed(string value) => !string.IsNullOrEmpty(value) && value.ToLower() == "ahmed";

    private bool CustomWithParentObject(Person person, string value) {
      return person.Age < 100
             && !string.IsNullOrEmpty(value) && value.ToLower() == "ahmed";
    }

    private bool CustomWithContextAndParentObject(Person person, string value, ValidationContext<Person> context) {
      var isValid = !string.IsNullOrEmpty(value) && value.ToLower() == "ahmed";
      if (!isValid) { context.AddFailure(nameof(Person.FirstName), "He is not Ahmed"); }

      return isValid;
    }
  }
}