using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.DynamicRules.Validators;
using FluentValidation.Validators;

namespace FluentValidation.DynamicRules.Extensions;

public static class RuleMethodHelper {
  public static MethodInfo? GetNotNullValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotNull), propType, Type.EmptyTypes);

  public static MethodInfo? GetNullValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Null), propType, Type.EmptyTypes);

  public static MethodInfo? GetNotEmptyValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotEmpty), propType, Type.EmptyTypes);

  public static MethodInfo? GetEmptyValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Empty), propType, Type.EmptyTypes);

  public static MethodInfo? EmailAddressValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.EmailAddress), propType,
      new[] { typeof(EmailValidationMode) });

  public static MethodInfo? GetCreditCardValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.CreditCard), propType, Type.EmptyTypes);

  public static MethodInfo? GetLengthValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Length), propType,
      new[] { typeof(int), typeof(int) });
  
  public static MethodInfo? GetExclusiveBetweenValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.ExclusiveBetween), propType,
      new[] { propType, propType });
  public static MethodInfo? GetInclusiveBetweenValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.InclusiveBetween), propType,
      new[] { propType, propType });

  public static MethodInfo? GetMinLengthValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.MinimumLength), propType,
      new[] { typeof(int) });

  public static MethodInfo? GetMaxLengthValidator(this Type validatedType, Type propType) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.MaximumLength), propType,
      new[] { typeof(int) });

  public static MethodInfo? GetNotEqualValidator(this Type validatedType, Type propType) {
    var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(propType);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotEqual), propType, new[] {
      propType,
      comparerGenericType
    });
  }

  public static MethodInfo? GetEqualValidator(this Type validatedType, Type propType) {
    var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(propType);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Equal), propType, new[] {
      propType,
      comparerGenericType
    });
  }

  public static MethodInfo? GetLessThanValidator(this Type validatedType, Type propType) {
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.LessThan), propType, new[] {
      propType
    });
  }

  public static MethodInfo? GetLessThanOrEqualValidator(this Type validatedType, Type propType) {
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.LessThanOrEqualTo), propType, new[] {
      propType
    });
  }

  public static MethodInfo? GetGreaterThanValidator(this Type validatedType, Type propType) {
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.GreaterThan), propType, new[] {
      propType
    });
  }

  public static MethodInfo? GetGreaterThanOrEqualValidator(this Type validatedType, Type propType) {
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.GreaterThanOrEqualTo), propType, new[] {
      propType
    });
  }

  public static MethodInfo? GetNotEqualValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(propType);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.NotEqual), propType, new[] {
      otherPropExp,
      comparerGenericType
    });
  }

  public static MethodInfo? GetEqualValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(propType);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Equal), propType, new[] {
      otherPropExp,
      comparerGenericType
    });
  }

  public static MethodInfo? GetLessThanValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.LessThan), propType, new[] {
      otherPropExp
    });
  }

  public static MethodInfo? GetLessThanOrEqualValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.LessThanOrEqualTo), propType, new[] {
      otherPropExp
    });
  }

  public static MethodInfo? GetGreaterThanValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.GreaterThan), propType, new[] {
      otherPropExp
    });
  }

  public static MethodInfo? GetGreaterThanOrEqualValidatorWithAnotherProperty(this Type validatedType, Type propType) {
    var func = typeof(Func<,>).MakeGenericType(validatedType, propType);
    var otherPropExp = typeof(Expression<>).MakeGenericType(func);
    return GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.GreaterThanOrEqualTo), propType, new[] {
      otherPropExp
    });
  }

  public static MethodInfo? GetPredicateValidator(this Type validatedType, Type propType, params Type[] paramTypes) =>
    GetValidationMethod(validatedType, nameof(DefaultValidatorExtensions.Must), propType, paramTypes);


  public static MethodInfo? GetWithMessageMethod<T>(this Type validatedType, Type propType) {
    var messageMethodParams = new[] { validatedType, typeof(string) };
    var method = typeof(DefaultValidatorOptions).GetStaticMethodForType(nameof(DefaultValidatorOptions.WithMessage),
      messageMethodParams);
    return method!.MakeGenericMethod(typeof(T), propType);
  }


  public static MethodInfo? GetValidationMethod(this Type validatedType, string methodName, Type propType, Type[]
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

  public static ConstantExpression GetRuleFor<T>(this AbstractValidator<T> validator, ParameterExpression pArg,
    ValidatedProperty
      prop) {
    var validatedType = typeof(T);
    var propExp = Expression.PropertyOrField(pArg, prop.PropertyName);
    var propType = validatedType.GetPropertyType(prop.PropertyName);
    var genericType = typeof(Func<,>).MakeGenericType(typeof(T), propType);
    var lambda = Expression.Lambda(genericType, propExp, pArg);

    var ruleFor = validator.GetType().BaseType!.GetMethod("RuleFor")!.MakeGenericMethod(propType);
    var result = ruleFor.Invoke(validator, new object?[] { lambda });
    return Expression.Constant(result);
  }
}