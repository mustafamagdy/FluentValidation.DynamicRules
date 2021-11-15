# Creating your first validator

To define rules for an object, you can implement your own parser to parse from any serialized rule definition to a set of `ValidatedProperty`. Or you can use the xml based provided one in this package.

In this documentation, I tried to copy the same structure as `FluentValidation` to make it easy for the ready to find what need to be added to make the validation works as expected.

For example, imagine that you have a Customer class:

```csharp
public class Customer 
{
  public int Id { get; set; }
  public string Surname { get; set; }
  public string Forename { get; set; }
  public decimal Discount { get; set; }
  public string Address { get; set; }
}
```

Instead of inheriting from `AbstractValidator<Customer>` you will inherit from `AbstractDynamicValidator<Customer>`, which has a default constructor that takes `ValidattionBuilder` instance, and pass it down to its base class.

```csharp
using FluentValidation;

public class CustomerValidator : AbstractDynamicValidator<Customer> 
{
    public CustomerValidator(ValidationBuilder<Customer> builder) : base(builder) {  }
}
```

The validation rules themselves should be defined in any `xml` string.

Then you pass the `xml` rules to a `RuleParser`.`Parse()` method and get back an instance of `ValidationBuilder<T>`.

Here is an example for the same `Customer` class

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

```c#
    var xml = "";//Get Customer xml rules
    var parser = new RuleParser();
    var builder = parser.Parse(xml);
    var customerValidator = new CustomerValidator(builder);
```

From now on, everything is handled by `FluentValidation` as is.
