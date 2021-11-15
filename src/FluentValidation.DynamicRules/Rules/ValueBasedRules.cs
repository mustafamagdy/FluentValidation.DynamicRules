namespace FluentValidation.DynamicRules.Rules;

internal abstract class ValueBasedRules : PropertyRule {
  protected ValueBasedRules(RuleType rule, string message, object? value, string? anotherProp = null)
    : base(rule, message) {
    Value = value;
    AnotherProp = anotherProp;
  }

  public object? Value { get; }
  public string? AnotherProp { get; }
}