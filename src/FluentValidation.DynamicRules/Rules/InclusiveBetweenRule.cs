namespace FluentValidation.DynamicRules.Rules;

internal sealed class InclusiveBetweenRule : RangeBasedRule {
  public InclusiveBetweenRule(string message, int min, int max) : base(RuleType.InclusiveBetween, message, min, max) { }
}