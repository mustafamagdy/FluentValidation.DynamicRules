namespace FluentValidation.DynamicRules.Rules;

internal sealed class EmailAddressRule : PropertyRule {
  public EmailAddressRule(string message) : base(RuleType.EmailAddress, message) { }
}