namespace FluentValidation.DynamicRules.Rules;

internal sealed class MinLengthRule : ValueBasedRules {
  public MinLengthRule(string message, object? value) : base(RuleType.MinLength, message, value) { }
}