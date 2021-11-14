using FluentValidation;

namespace FluentValidation.DynamicRules.Validators;

public abstract class AbstractDynamicValidator<T> : AbstractValidator<T> {
  protected AbstractDynamicValidator(ValidationBuilder<T> builder) { builder.BuildFor(this); }
}