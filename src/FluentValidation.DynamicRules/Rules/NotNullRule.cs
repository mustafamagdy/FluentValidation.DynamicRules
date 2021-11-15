namespace FluentValidation.DynamicRules.Rules;

internal sealed class NotNullRule : PropertyRule {
  public NotNullRule(string message) : base(RuleType.NotNull, message) { }
}