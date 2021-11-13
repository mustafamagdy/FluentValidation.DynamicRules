namespace FluentValidation.DynamicRules.Rules;

public sealed class MinLengthRule : ValueBasedRules {
  public MinLengthRule(string message, object? value) : base(RuleType.MinLength, message, value) { }
}