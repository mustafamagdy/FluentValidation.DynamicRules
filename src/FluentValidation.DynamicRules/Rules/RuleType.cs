namespace FluentValidation.DynamicRules.Rules;

public enum RuleType {
  Empty,
  Null,
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
  EmailAddress,
  CreditCard,
  ExclusiveBetween,
  InclusiveBetween
}