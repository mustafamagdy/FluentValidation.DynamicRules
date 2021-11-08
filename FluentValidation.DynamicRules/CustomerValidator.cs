using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules;

public class CustomerValidator : AbstractDynamicValidator<Customer> {
  public CustomerValidator(ValidationBuilder builder) : base(builder) {
//
  }

  private bool BeValidPostalCode(string postalCode) {
    if (postalCode == "test")
      return true;
    return false;
  }
}