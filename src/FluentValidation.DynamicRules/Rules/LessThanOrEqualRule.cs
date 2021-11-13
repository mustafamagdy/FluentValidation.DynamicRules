namespace FluentValidation.DynamicRules.Rules;

public sealed class LessThanOrEqualRule : ValueBasedRules {
  public LessThanOrEqualRule(string message, object? value, string? anotherProp = null) : base(message, value,
    anotherProp) {
  }
}