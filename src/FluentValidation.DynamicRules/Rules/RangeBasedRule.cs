namespace FluentValidation.DynamicRules.Rules;

internal abstract class RangeBasedRule : PropertyRule {
  protected RangeBasedRule(RuleType rule, string message, int min, int max) : base(rule, message) {
    Min = min;
    Max = max;
  }

  public int Min { get; }
  public int Max { get; }

  public void Deconstruct(out int min, out int max) {
    min = Min;
    max = Max;
  }
}