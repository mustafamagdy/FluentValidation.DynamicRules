namespace FluentValidation.DynamicRules.Rules;

public sealed class NotEqualRule : PropertyRule {
  public NotEqualRule(string message, object? value, string? anotherProp = null) : base(RuleType.NotEqual, message) {
    Value = value;
    AnotherProp = anotherProp;
  }

  public object? Value { get; }
  public string? AnotherProp { get; }
}