namespace FluentValidation.DynamicRules.Rules;

public sealed class GreaterThanOrEqualRule : ValueBasedRules {
  public GreaterThanOrEqualRule(string message, object? value, string? anotherProp = null) 
    : base(RuleType.GreaterThanOrEqual, message, value, anotherProp) {
  }
}