namespace FluentValidation.DynamicRules.Rules;

public sealed class EmailAddressRule : PropertyRule {
  public EmailAddressRule(string message) : base(RuleType.EmailAddress, message) { }
}