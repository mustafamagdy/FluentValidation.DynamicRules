using FluentValidation.DynamicRules.Rules;

namespace FluentValidation.DynamicRules.Rules;

public abstract class PropertyRule: IPropertyRule {
  protected PropertyRule(RuleType ruleType, string message) {
    RuleType = ruleType;
    Message = message;
  }

  public RuleType RuleType { get; }
  public string Message { get; }
}