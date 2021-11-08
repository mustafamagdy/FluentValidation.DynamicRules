using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules;

public class CustomerValidator : AbstractDynamicValidator<Customer> {
  public CustomerValidator(ValidationBuilder builder) : base(builder) { }

  private bool BeValidPostalCode(string postalCode) {
    if (postalCode == "12345")
      return true;
    return false;
  }
}