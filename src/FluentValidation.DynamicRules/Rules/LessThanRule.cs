namespace FluentValidation.DynamicRules.Rules;

public sealed class LessThanRule : ValueBasedRules {
  public LessThanRule(string message, object? value, string? anotherProp = null) : base(message, value,
    anotherProp) {
  }
}