namespace FluentValidation.DynamicRules.Rules;

internal sealed class EmptyRule : PropertyRule {
  public EmptyRule(string message) : base(RuleType.Empty, message) { }
}