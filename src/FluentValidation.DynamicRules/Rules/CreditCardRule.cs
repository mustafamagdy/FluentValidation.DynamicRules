namespace FluentValidation.DynamicRules.Rules;

public sealed class CreditCardRule : PropertyRule {
  public CreditCardRule(string message) : base(RuleType.CreditCard, message) { }
}