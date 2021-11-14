using System.Collections;
using System.Collections.Generic;
using Bogus;
using FluentValidation.DynamicRules.Validators;
using Xunit;

namespace FluentValidation.DynamicRules.Tests.Validators {
  public class TestObjectHelper {
    public static Faker _faker = new();

    public static MainObject Item1 => new() {
      Text1 = _faker.Random.Word(),
      Text2 = _faker.Random.Word(),
      Text3 = _faker.Random.Word(),
    };

    public static MainObject ItemWithSubItem => new() {
      Text1 = _faker.Random.Word(),
      Text2 = _faker.Random.Word(),
      Text3 = _faker.Random.Word(),
      SubItems = new List<SubObject>() {
        new() { Int1 = 1 }
      }
    };
  }

  public class MainObject {
    public string Text1 { get; set; }
    public string Text2 { get; set; }
    public string Text3 { get; set; }

    public List<SubObject> SubItems { get; set; }
  }

  public class SubObject {
    public int Int1 { get; set; }
  }

  public class MainObjectValidator : AbstractDynamicValidator<MainObject> {
    public MainObjectValidator(ValidationBuilder<MainObject> builder) : base(builder) {
    }
  }

  public class NotNullTests {
    [Fact]
    public void Parser_Parse_String_NotNull() {
      var parser = new RuleParser<MainObject>();
      var xml = @"<rules><rule-for prop=""firstName""><not-null message=""value cannot be null""/></rule-for></rules>";
      var builder = parser.Parse(xml);
      var validator = new MainObjectValidator(builder);

      var obj1 = TestObjectHelper.Item1;
      var result1 = validator.Validate(obj1);
      Assert.False(result1.IsValid);
      Assert.NotEmpty(result1.Errors);
      Assert.Equal(nameof(DynamicRuleTests.Person.FirstName), result1.Errors[0].PropertyName);
      Assert.Equal("value cannot be null", result1.Errors[0].ErrorMessage);
      Assert.Equal("NotNullValidator", result1.Errors[0].ErrorCode);
    }

    [Theory]
    [MemberData(nameof(TestData.GetInValidCases), MemberType = typeof(TestData))]
    public void InValidate_Cases(string xml, MainObject item, string propertyName, string errorMessage, string errorCode) {
      var parser = new RuleParser<MainObject>();
      var builder = parser.Parse(xml);
      var validator = new MainObjectValidator(builder);

      var result1 = validator.Validate(item);
      Assert.False(result1.IsValid);
      Assert.NotEmpty(result1.Errors);
      Assert.Equal(propertyName, result1.Errors[0].PropertyName);
      Assert.Equal(errorMessage, result1.Errors[0].ErrorMessage);
      Assert.Equal(errorCode, result1.Errors[0].ErrorCode);
    }
  }

  public class TestData {
    public static IEnumerable<object[]> GetInValidCases() {
      yield return new object[] {
        @"<rules><rule-for prop=""subitems""><not-null /></rule-for></rules>",
        new MainObject(),
        nameof(MainObject.SubItems),
        "'Sub Items' must not be empty.",
        "NotNullValidator"
      };
      yield return new object[] {
        @"<rules><rule-for prop=""subitems""><not-null /></rule-for></rules>",
        new MainObject { SubItems = null },
        nameof(MainObject.SubItems),
        "'Sub Items' must not be empty.",
        "NotNullValidator"
      };

      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><not-null /></rule-for></rules>",
        new MainObject(),
        nameof(MainObject.Text1),
        "'Text1' must not be empty.",
        "NotNullValidator"
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><not-empty /></rule-for></rules>",
        new MainObject(),
        nameof(MainObject.Text1),
        "'Text1' must not be empty.",
        "NotEmptyValidator"
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><min-len value=""10"" /></rule-for></rules>",
        new MainObject(){Text1 = " "},
        nameof(MainObject.Text1),
        "The length of 'Text1' must be at least 10 characters. You entered 1 characters.",
        "MinimumLengthValidator"
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><min-len value=""10"" /></rule-for></rules>",
        new MainObject { Text1 = "123456789" },
        nameof(MainObject.Text1),
        "The length of 'Text1' must be at least 10 characters. You entered 9 characters.",
        "MinimumLengthValidator"
      };
    }
  }
}