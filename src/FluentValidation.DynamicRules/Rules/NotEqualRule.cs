namespace FluentValidation.DynamicRules.Rules;

public sealed class NotEqualRule : ValueBasedRules {
  public NotEqualRule(string message, object? value, string? anotherProp = null) : base(message, value,
    anotherProp) {
  }
}