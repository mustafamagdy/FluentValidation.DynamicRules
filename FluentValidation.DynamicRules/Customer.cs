namespace FluentValidation.DynamicRules;

public class Customer {
  public string FirstName { get; set; }
  public string Address { get; set; }
  public bool HasDiscount { get; set; }
  public int Discount { get; set; }
  public string PostalCode { get; set; }
}