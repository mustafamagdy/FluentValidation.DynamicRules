namespace FluentValidation.DynamicRules.Rules;

public sealed class GreaterThanRule : ValueBasedRules {
  public GreaterThanRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.GreaterThan, message, value,  anotherProp) {
  }
}