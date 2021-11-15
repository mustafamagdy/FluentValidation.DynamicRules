namespace FluentValidation.DynamicRules.Rules;

internal sealed class CreditCardRule : PropertyRule {
  public CreditCardRule(string message) : base(RuleType.CreditCard, message) { }
}