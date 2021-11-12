namespace FluentValidation.DynamicRules.Rules;

public sealed class LengthRule : PropertyRule {
  public LengthRule(string message, int min, int max) : base(RuleType.Length, message) {
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