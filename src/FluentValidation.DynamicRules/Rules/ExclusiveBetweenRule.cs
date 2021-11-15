namespace FluentValidation.DynamicRules.Rules;

internal sealed class ExclusiveBetweenRule : RangeBasedRule {
  public ExclusiveBetweenRule(string message, int min, int max) : base(RuleType.ExclusiveBetween, message, min, max) { }
}