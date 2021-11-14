namespace FluentValidation.DynamicRules.Rules;

internal sealed class LessThanRule : ValueBasedRules {
  public LessThanRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.LessThanOrEqual, message, value, anotherProp) {
  }
}