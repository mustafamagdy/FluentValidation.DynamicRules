# Dynamic Rules for FluentValidation

Add support to dynamically loading validation rules from xml based rule definition

```xml
<rules>
    <rule-for prop="firstName">
      <not-empty message="value cannot be empty" />
    </rule-for>
    <rule-for prop="address">
      <string-len min="10" max="20" />
    </rule-for>
    <rule-for prop="discount" type="System.Int32">
      <not-equal value="0"/>
    </rule-for>
    <rule-for prop="postalCode">
      <must-be call="BeValidPostalCode" message="postal code is not valid" />
    </rule-for>
</rules>
```
