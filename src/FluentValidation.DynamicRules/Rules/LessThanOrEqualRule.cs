namespace FluentValidation.DynamicRules.Rules;

public sealed class LessThanOrEqualRule : ValueBasedRules {
  public LessThanOrEqualRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.LessThanOrEqual, message, value, anotherProp) {
  }
}