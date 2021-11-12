using FluentValidation;

namespace FluentValidation.DynamicRules.Validators;

public abstract class AbstractDynamicValidator<T> : AbstractValidator<T> {
  public AbstractDynamicValidator(ValidationBuilder builder) {
    //
    builder.BuildFor(this);
  }
}