namespace FluentValidation.DynamicRules.Rules;

public sealed class NotEmptyRule : PropertyRule {
  public NotEmptyRule(string message) : base(RuleType.NotEmpty, message) { }
}