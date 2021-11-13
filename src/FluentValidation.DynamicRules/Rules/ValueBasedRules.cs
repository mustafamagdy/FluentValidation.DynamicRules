namespace FluentValidation.DynamicRules.Rules;

public abstract class ValueBasedRules : PropertyRule {
  protected ValueBasedRules(string message, object? value, string? anotherProp = null) : base(RuleType
      .GreaterThanOrEqual, 
    message) {
    Value = value;
    AnotherProp = anotherProp;
  }

  public object? Value { get; }
  public string? AnotherProp { get; }
}