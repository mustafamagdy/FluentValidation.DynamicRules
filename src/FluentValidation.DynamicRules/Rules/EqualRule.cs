namespace FluentValidation.DynamicRules.Rules;

public sealed class EqualRule : ValueBasedRules {
  public EqualRule(string message, object? value, string? anotherProp = null)
    : base(RuleType.Equal, message, value, anotherProp) {
  }
}