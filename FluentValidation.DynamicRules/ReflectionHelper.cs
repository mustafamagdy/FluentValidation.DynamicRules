using System;
using System.Linq;
using System.Reflection;

namespace FluentValidation.DynamicRules;

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

      var found = @params.Where((t, i) => FilterParams(t, i, paramTypes)).Any();
      if (!found) continue;

      foundMethod = m;
      break;
    }

    return foundMethod;
  }

  private static bool FilterParams(Type t, int i, Type[] paramTypes) {
    return (t.IsAssignableFrom(paramTypes[i])
            || (t.IsGenericParameter && paramTypes[i].IsGenericParameter)
            || (t.IsGenericTypeDefinition &&
                t.GetGenericTypeDefinition().IsAssignableFrom(paramTypes[i].GetGenericTypeDefinition())));
  }
}