namespace FluentValidation.DynamicRules.Rules;

public sealed class NotEqualRule : PropertyRule {
  public NotEqualRule(string message, object value, string? whenPropName = null) : base(RuleType.NotEqual, message) {
    Value = value;
    WhenPropName = whenPropName;
  }

  public object Value { get; }
  public string? WhenPropName { get; }

  public void Deconstruct(out object value) { value = Value; }
}