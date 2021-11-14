using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.DynamicRules.Validators;
using Xunit;

namespace FluentValidation.DynamicRules.Tests.Validators {
  public class NotNullTests {
    [Theory]
    [MemberData(nameof(TestData.GetCases), MemberType = typeof(TestData))]
    public void TestSinglePropertyValidation(string xml, MainObject item, bool isValid,
      string propertyName, string errorMessage, string errorCode) {
      var parser = new RuleParser<MainObject>();
      var builder = parser.Parse(xml);
      var validator = new MainObjectValidator(builder);
      var result1 = validator.Validate(item);

      Assert.Equal(isValid, result1.IsValid);
      if (!isValid) {
        Assert.NotEmpty(result1.Errors);
        Assert.Equal(propertyName, result1.Errors[0].PropertyName);
        Assert.Equal(errorMessage, result1.Errors[0].ErrorMessage);
        Assert.Equal(errorCode, result1.Errors[0].ErrorCode);
      } else {
        Assert.Empty(result1.Errors);
      }
    }
  }

  public class TestData {
    public static IEnumerable<object[]> GetCases() => GetValidCases().Concat(GetInvalidCases());

    private static IEnumerable<object[]> GetInvalidCases() {
      yield return new object[] {
        @"<rules><rule-for prop=""subItems""><not-null /></rule-for></rules>",
        new MainObject(),
        false,
        nameof(MainObject.SubItems),
        "'Sub Items' must not be empty.",
        "NotNullValidator",
      };
      yield return new object[] {
        @"<rules><rule-for prop=""subItems""><not-null /></rule-for></rules>",
        new MainObject { SubItems = null },
        false,
        nameof(MainObject.SubItems),
        "'Sub Items' must not be empty.",
        "NotNullValidator",
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><not-null /></rule-for></rules>",
        new MainObject(),
        false,
        nameof(MainObject.Text1),
        "'Text1' must not be empty.",
        "NotNullValidator",
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><not-empty /></rule-for></rules>",
        new MainObject(),
        false,
        nameof(MainObject.Text1),
        "'Text1' must not be empty.",
        "NotEmptyValidator",
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><min-len value=""10"" /></rule-for></rules>",
        new MainObject() { Text1 = " " },
        false,
        nameof(MainObject.Text1),
        "The length of 'Text1' must be at least 10 characters. You entered 1 characters.",
        "MinimumLengthValidator",
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Text1""><min-len value=""10"" /></rule-for></rules>",
        new MainObject { Text1 = "123456789" },
        false,
        nameof(MainObject.Text1),
        "The length of 'Text1' must be at least 10 characters. You entered 9 characters.",
        "MinimumLengthValidator",
      };
    }


    private static IEnumerable<object[]> GetValidCases() {
      yield return new object[] {
        @"<rules><rule-for prop=""Int1""><greater-than value=""100"" /></rule-for></rules>",
        new MainObject() { Int1 = 101 },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Int1""><greater-than-equal value=""100"" /></rule-for></rules>",
        new MainObject() { Int1 = 100 },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Int1""><less-than value=""100"" /></rule-for></rules>",
        new MainObject() { Int1 = 99 },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""Int1""><less-than-equal value=""100"" /></rule-for></rules>",
        new MainObject() { Int1 = 100 },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""text1""><not-null /></rule-for></rules>",
        new MainObject() { Text1 = "test" },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""text1""><min-len value=""4"" /></rule-for></rules>",
        new MainObject() { Text1 = "1234" },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""text1""><max-len value=""10"" /></rule-for></rules>",
        new MainObject() { Text1 = "1234567890" },
        true, null, null, null
      };
      yield return new object[] {
        @"<rules><rule-for prop=""text1""><not-empty /></rule-for></rules>",
        new MainObject() { Text1 = "test" },
        true, null, null, null
      };
    }
  }

  public class MainObject {
    public string Text1 { get; set; }
    public string Text2 { get; set; }
    public string Text3 { get; set; }
    public int Int1 { get; set; }

    public List<SubObject> SubItems { get; set; }
  }

  public class SubObject {
    public int Int1 { get; set; }
  }

  public class MainObjectValidator : AbstractDynamicValidator<MainObject> {
    public MainObjectValidator(ValidationBuilder<MainObject> builder) : base(builder) { }
  }
}