namespace FluentValidation.DynamicRules.Rules;

internal sealed class LessThanOrEqualRule : ValueBasedRules {
  public LessThanOrEqualRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.LessThanOrEqual, message, value, anotherProp) {
  }
}