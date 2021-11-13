namespace FluentValidation.DynamicRules.Rules;

public sealed class NotNullRule : PropertyRule {
  public NotNullRule(string message) : base(RuleType.NotNull, message) { }
}