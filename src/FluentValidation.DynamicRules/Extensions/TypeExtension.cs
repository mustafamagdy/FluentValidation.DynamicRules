using System;
using System.Linq;
using System.Reflection;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules.Extensions;

internal static class TypeExtension {
  public static object GetDefaultComparerForType(this Type type) {
    var defaultComparerType = typeof(DefaultEqualityComparer<>).MakeGenericType(type);
    var defaultComparer = defaultComparerType.New();
    return defaultComparer;
  }

  private static object New(this Type type) => Activator.CreateInstance(type)!;
}