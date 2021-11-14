namespace FluentValidation.DynamicRules.Rules;

internal sealed class MaxLengthRule : ValueBasedRules {
  public MaxLengthRule(string message, object? value) : base(RuleType.MaxLength, message, value) { }
}