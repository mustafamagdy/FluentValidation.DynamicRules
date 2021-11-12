# Dynamic Rules for [FluentValidation](https://fluentvalidation.net/)

Add support to dynamically loading validation rules from xml based rule definition

![main](https://github.com/mustafamagdy/FluentValidation.DynamicRules/actions/workflows/build.yml/badge.svg)

## Setup
```c# 
    public class CustomerValidator : AbstractDynamicValidator<Customer> {
      public CustomerValidator(ValidationBuilder builder) : base(builder) { }
    
      private bool BeValidPostalCode(string postalCode) {
        // custom postcode validating logic goes here
      }
    }
```

### Construct the builder
```c#
    // Some where in your setup
    var parser = new RuleParser();
    var builder = parser.Parse(xml);
    var customerValidator = new CustomerValidator(builder);
```
### Sample XML rules

```xml
<rules>
    <rule-for prop="firstName">
      <not-empty message="Please specify a first name" />
    </rule-for>
    <rule-for prop="address">
      <string-len min="20" max="250" />
    </rule-for>
    <rule-for prop="discount">
      <not-equal value="0"/>
    </rule-for>
    <rule-for prop="postalCode">
      <must-be call="BeValidPostalCode" message="Please specify a valid postcode" />
    </rule-for>
</rules>
```

This is equivalent to:
```c#
public class CustomerValidator : AbstractValidator<Customer> {
  public CustomerValidator() {
    RuleFor(x => x.Surname).NotEmpty();
    RuleFor(x => x.Forename).NotEmpty().WithMessage("Please specify a first name");
    RuleFor(x => x.Discount).NotEqual(0);
    RuleFor(x => x.Address).Length(20, 250);
    RuleFor(x => x.Postcode).Must(BeAValidPostcode).WithMessage("Please specify a valid postcode");
  }

  private bool BeAValidPostcode(string postcode) {
    // custom postcode validating logic goes here
  }
}
```