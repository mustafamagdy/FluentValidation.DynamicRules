using System;
using System.Linq;
using System.Reflection;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules.Extensions;

public static class TypeExtension {
  public static object GetDefaultComparerForType(this Type type) {
    var defaultComparerType = typeof(DefaultEqualityComparer<>).MakeGenericType(type);
    var defaultComparer = defaultComparerType.GetDefaultComparerForType();
    return defaultComparer;
  }
}