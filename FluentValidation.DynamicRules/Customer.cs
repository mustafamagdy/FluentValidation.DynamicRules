namespace FluentValidation.DynamicRules;

public class Customer {
  public string FirstName { get; init; }
  public string Address { get; init; }
  public bool HasDiscount { get; init; }
  public int Discount { get; init; }
  public string PostalCode { get; init; }
}