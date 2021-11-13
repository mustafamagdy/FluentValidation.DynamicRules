using System;
using System.Linq;
using System.Reflection;

namespace FluentValidation.DynamicRules.Extensions;

public static class ReflectionHelper {
  public static MethodInfo? GetStaticMethodForType(this Type type, string methodName, params Type[] paramTypes) {
    return GetMethodForType(type, methodName,
      BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, paramTypes);
  }

  public static MethodInfo? GetPublicMethodForType(this Type type, string methodName, params Type[] paramTypes) {
    return GetMethodForType(type, methodName,
      BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, paramTypes);
  }

  public static MethodInfo? GetPrivateMethodForType(this Type type, string methodName, params Type[] paramTypes) {
    return GetMethodForType(type, methodName,
      BindingFlags.NonPublic | BindingFlags.Instance, paramTypes);
  }

  public static Type GetPropertyType(this Type type, string propName) {
    return type
      .GetProperty(propName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public)?
      .PropertyType!;
  }


  private static MethodInfo? GetMethodForType(this Type type, string methodName, BindingFlags flags,
    params Type[] paramTypes) {
    MethodInfo?[] methods = type
      .GetMethods(flags)
      .Where(method => method.Name == methodName && method.GetParameters().Length == paramTypes.Length)
      .ToArray();

    MethodInfo? foundMethod = null;

    foreach (var m in methods) {
      var @params = m!.GetParameters()
        .Select(parameter => parameter.ParameterType)
        .Select(t => t.IsGenericType ? t.GetGenericTypeDefinition() : t)
        .ToArray();

      var found = @params.Select((t, i) => FilterParams(t, paramTypes[i])).All(a => a);
      if (!found) continue;

      foundMethod = m;
      break;
    }

    return foundMethod;
  }

  private static bool FilterParams(Type t, Type paramType) {
    return (t.IsAssignableFrom(paramType)
            || (t.IsGenericParameter && !paramType.IsGenericType
                                     && (t.GetGenericParameterConstraints().Length == 0
                                         || t.GetGenericParameterConstraints().Contains(paramType)))
            || (t.IsGenericType && paramType.IsGenericType &&
                t.GetGenericTypeDefinition().IsAssignableFrom(paramType.GetGenericTypeDefinition())));
  }
}