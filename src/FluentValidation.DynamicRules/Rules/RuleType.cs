namespace FluentValidation.DynamicRules.Rules;

public enum RuleType {
  NotNull,
  NotEmpty,
  Equal,
  NotEqual,
  LessThan,
  LessThanOrEqual,
  GreaterThan,
  GreaterThanOrEqual,
  Length,
  MinLength,
  MaxLength,
  MustBe,
}