namespace FluentValidation.DynamicRules.Rules;

internal sealed class NotEmptyRule : PropertyRule {
  public NotEmptyRule(string message) : base(RuleType.NotEmpty, message) { }
}