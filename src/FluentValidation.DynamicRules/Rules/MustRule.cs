namespace FluentValidation.DynamicRules.Rules;

public sealed class MustRule : PropertyRule {
  public MustRule(string message, string methodName, string methodWithParent, string methodWithContext)
    : base(RuleType.MustBe, message) {
    MethodName =
      methodName;
    MethodWithParent = methodWithParent;
    MethodWithContext = methodWithContext;
  }

  public string MethodName { get; }
  public string MethodWithParent { get; }
  public string MethodWithContext { get; }

  public void Deconstruct(out string methodName, out string methodWithParent, out string methodWithContext) {
    methodName = MethodName;
    methodWithParent = MethodWithParent;
    methodWithContext = MethodWithContext;
  }
}