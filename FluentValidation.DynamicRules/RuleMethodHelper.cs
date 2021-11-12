using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules;

public static class RuleMethodHelper {
  public static MethodInfo? GetValidationMethod(this Type validatedType, string methodName, Type propType, params Type[]
    paramTypes) {
    var methodParamTypes = new List<Type> { typeof(IRuleBuilder<,>) };
    methodParamTypes.AddRange(paramTypes);
    var method = typeof(DefaultValidatorExtensions).GetPublicMethodForType(methodName, methodParamTypes.ToArray())
                 ?? throw new NullReferenceException($"Unable to find method {methodName} in type " +
                                                     $"{nameof(DefaultValidatorExtensions)}");
    return method!.GetGenericArguments().Length == 2
      ? method.MakeGenericMethod(validatedType, propType)
      : method.MakeGenericMethod(validatedType);
  }


  public static Expression GetMessageMethodCall<T>(this AbstractValidator<T> validator) {
    var mustMethod = validator.GetType().GetPrivateMethodForType(nameof(DefaultValidatorOptions.WithMessage),
      typeof(string));
    
    var arg01 = Expression.Parameter(typeof(string));
    var messageMethodCall = Expression.Call(Expression.Constant(validator), mustMethod!, arg01);
    return Expression.Lambda(messageMethodCall, arg01);
  }

  public static ConstantExpression GetRuleFor<T>(this AbstractValidator<T> validator, ValidatedProperty prop) {
    var validatedType = typeof(T);
    var pT = Expression.Parameter(validatedType);
    var propExp = Expression.PropertyOrField(pT, prop.PropertyName);
    var propType = validatedType.GetPropertyType(prop.PropertyName);
    var genericType = typeof(Func<,>).MakeGenericType(typeof(T), propType);
    var lambda = Expression.Lambda(genericType, propExp, pT);

    var ruleFor = validator.GetType().BaseType!.GetMethod("RuleFor")!.MakeGenericMethod(propType);
    var result = ruleFor.Invoke(validator, new object?[] { lambda });
    return Expression.Constant(result);
  }

  public static object GetDefaultComparerForType(this Type type) => Activator.CreateInstance(type)!;
}