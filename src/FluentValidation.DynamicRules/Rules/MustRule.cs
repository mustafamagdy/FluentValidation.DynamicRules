namespace FluentValidation.DynamicRules.Rules;

public sealed class MustRule : PropertyRule {
  public MustRule(string message, string methodName) : base(RuleType.MustBe, message) { MethodName = methodName; }
  public string MethodName { get; }
}