namespace FluentValidation.DynamicRules.Rules;

public sealed class EmptyRule : PropertyRule {
  public EmptyRule(string message) : base(RuleType.Empty, message) { }
}