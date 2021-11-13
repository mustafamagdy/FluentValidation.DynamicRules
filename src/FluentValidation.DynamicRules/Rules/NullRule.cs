namespace FluentValidation.DynamicRules.Rules;

public sealed class NullRule : PropertyRule {
  public NullRule(string message) : base(RuleType.Null, message) { }
}