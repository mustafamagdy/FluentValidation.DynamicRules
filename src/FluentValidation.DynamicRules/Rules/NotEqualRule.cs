using System.Data;

namespace FluentValidation.DynamicRules.Rules;

internal sealed class NotEqualRule : ValueBasedRules {
  public NotEqualRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.NotEqual, message, value, anotherProp) {
  }
}