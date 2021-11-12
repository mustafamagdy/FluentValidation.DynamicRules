namespace FluentValidation.DynamicRules.Rules;

public abstract class PropertyRule {
  protected PropertyRule(RuleType ruleType, string message) {
    RuleType = ruleType;
    Message = message;
  }

  public RuleType RuleType { get; }
  public string Message { get; }
}