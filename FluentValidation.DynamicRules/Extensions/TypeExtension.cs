using System;
using System.Linq;
using System.Reflection;

namespace FluentValidation.DynamicRules.Extensions;

public static class TypeExtension {
  public static MethodInfo? GetStaticMethodForType(this Type staticType, string methodName, params Type[] paramTypes) {
    return GetStaticMethodForType(staticType, methodName,
      BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static, paramTypes);
  }

  public static MethodInfo? GetStaticMethodForType(this Type staticType, string methodName, BindingFlags flags,
    params Type[] paramTypes) {
    MethodInfo?[] methods = (from method in staticType.GetMethods(flags)
      where method.Name == methodName
            && method.GetParameters().Length == paramTypes.Length
      select method).ToArray();

    MethodInfo? foundMethod = null;

    foreach (var m in methods) {
      var @params = m.GetParameters()
        .Select(parameter => parameter.ParameterType)
        .Select(type => type.IsGenericType ? type.GetGenericTypeDefinition() : type)
        .ToArray();

      var found = !@params
        .Where((t, i) => !t.IsAssignableFrom(paramTypes[i])
                         && (!t.IsGenericParameter || paramTypes[i].IsGenericParameter)
                         && (!t.IsGenericTypeDefinition
                             || !t.GetGenericTypeDefinition()
                               .IsAssignableFrom(paramTypes[i].GetGenericTypeDefinition()))
        ).Any();

      if (!found) continue;

      foundMethod = m;
      break;
    }

    return foundMethod;
  }
}