namespace FluentValidation.DynamicRules.Rules;

public sealed class GreaterThanRule : ValueBasedRules {
  public GreaterThanRule(string message, object? value, string? anotherProp = null) : base(message, value,
    anotherProp) {
  }
}