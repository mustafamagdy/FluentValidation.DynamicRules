namespace FluentValidation.DynamicRules.Rules;

public sealed class InclusiveBetweenRule : RangeBasedRule {
  public InclusiveBetweenRule(string message, int min, int max) : base(RuleType.InclusiveBetween, message, min, max) { }
}