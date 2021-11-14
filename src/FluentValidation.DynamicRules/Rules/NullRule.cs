namespace FluentValidation.DynamicRules.Rules;

internal sealed class NullRule : PropertyRule {
  public NullRule(string message) : base(RuleType.Null, message) { }
}