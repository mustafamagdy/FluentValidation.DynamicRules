using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules;

public static class RuleMethodHelper {
  public static MethodInfo? GetNotEmptyValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotEmpty), propType);

  public static MethodInfo? GetLengthValidator(this Type validatedType, Type propType, params Type[] paramTypes) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Length), propType, paramTypes);

  public static MethodInfo? GetNotEqualValidator(this Type validatedType, Type propType) {
    var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(propType);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotEqual), propType, propType,
      comparerGenericType);
  }

  public static MethodInfo? GetPredicateValidator(this Type validatedType, Type propType, params Type[] paramTypes) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Must), propType, paramTypes);


  public static MethodInfo? GetWithMessageMethod<T>(this Type validatedType, Type propType) {
    var messageMethodParams = new[] { validatedType, propType };
    var method = typeof(DefaultValidatorOptions).GetStaticMethodForType(nameof(DefaultValidatorOptions.WithMessage),
      messageMethodParams);
    return method!.MakeGenericMethod(typeof(T), propType);
  }


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

  
}