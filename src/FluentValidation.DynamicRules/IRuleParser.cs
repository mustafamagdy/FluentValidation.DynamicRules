using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules;

public interface IRuleParser<T> {
  ValidationBuilder<T> Parse(string text);
}