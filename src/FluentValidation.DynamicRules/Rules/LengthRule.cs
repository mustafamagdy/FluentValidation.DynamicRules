namespace FluentValidation.DynamicRules.Rules;

internal sealed class LengthRule : RangeBasedRule {
  public LengthRule(string message, int min, int max) : base(RuleType.Length, message, min, max) { }
}