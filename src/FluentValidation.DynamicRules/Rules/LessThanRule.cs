namespace FluentValidation.DynamicRules.Rules;

public sealed class LessThanRule : ValueBasedRules {
  public LessThanRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.LessThanOrEqual, message, value, anotherProp) {
  }
}