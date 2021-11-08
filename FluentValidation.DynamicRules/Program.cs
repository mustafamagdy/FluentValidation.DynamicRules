using System;

namespace FluentValidation.DynamicRules {
  public static class Program {
    public static void Main() {
      const string xml = @"
<rules>
    <rule-for prop=""firstName"">
      <not-empty message=""value cannot be empty"" />
      <string-len min=""10"" max=""20"" />
   </rule-for>
    <rule-for prop=""address"">
      <string-len min=""10"" max=""20"" />
    </rule-for>
    <rule-for prop=""discount"">
      <not-equal value=""0""/>
    </rule-for>
    <rule-for prop=""postalCode"">
      <must-be call=""BeValidPostalCode"" message=""postal code is not valid!!!"" />
    </rule-for>
</rules>";
      
      var parser = new RuleParser();
      var builder = parser.Parse(xml);
      var customerValidator = new CustomerValidator(builder);
      var customer1 = new Customer() {
        FirstName = "test",
        Address = "test address here",
        Discount = 0,
        PostalCode = ""
      };

      var validationResult = customerValidator.Validate(customer1);
      
      Console.ReadLine();
    }
  }
}