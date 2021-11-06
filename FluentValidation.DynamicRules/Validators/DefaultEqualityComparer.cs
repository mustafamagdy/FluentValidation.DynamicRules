using System.Collections.Generic;

namespace FluentValidation.DynamicRules.Validators;

public class DefaultEqualityComparer<T> : IEqualityComparer<T> {
  public bool Equals(T? x, T? y) => x.Equals(y);
  public int GetHashCode(T obj) => obj.GetHashCode();
}