namespace FluentValidation.DynamicRules.Rules;

public sealed class ExclusiveBetweenRule : RangeBasedRule {
  public ExclusiveBetweenRule(string message, int min, int max) : base(RuleType.ExclusiveBetween, message, min, max) { }
}