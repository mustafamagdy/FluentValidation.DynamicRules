namespace FluentValidation.DynamicRules.Rules;

public sealed class LengthRule : RangeBasedRule {
  public LengthRule(string message, int min, int max) : base(RuleType.Length, message, min, max) { }
}