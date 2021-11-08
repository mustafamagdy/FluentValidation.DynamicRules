using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.DynamicRules.Extensions;
using FluentValidation;
using FluentValidation.DynamicRules.Rules;

namespace FluentValidation.DynamicRules.Validators;

public class ValidationBuilder {
  private readonly IEnumerable<ValidatedProperty> _properties;
  public ValidationBuilder(IEnumerable<ValidatedProperty> properties) { _properties = properties; }

  public void BuildFor<T>(AbstractValidator<T> validator) {
    _properties.ForEach(prop => BuildRulesForProperty(prop, validator));
  }

  private void BuildRulesForProperty<T>(ValidatedProperty prop, AbstractValidator<T> validator) {
    prop.Rules.ForEach(rule => BuildRule(rule, prop, validator));
  }

  private void BuildRule<T>(PropertyRule rule, ValidatedProperty prop, AbstractValidator<T> validator) {
    var ruleFor = GetRuleFor(validator, prop);
    MethodInfo? method;
    MethodCallExpression methodCall;
    switch (rule.RuleType) {
      case RuleType.NotEmpty: {
        method = GetValidationMethod<T>(nameof(DefaultValidatorExtensions.NotEmpty), prop.Type);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.Length: {
        var (min, max) = (LengthRule)rule;
        method = GetValidationMethod<T>(nameof(DefaultValidatorExtensions.Length), prop.Type, typeof(int), typeof(int));
        methodCall = Expression.Call(null, method!, ruleFor, Expression.Constant(min), Expression.Constant(max));
        break;
      }
      case RuleType.NotEqual: {
        var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(prop.Type);
        var defaultComparerType = typeof(DefaultEqualityComparer<>).MakeGenericType(prop.Type);
        var defaultComparer = GetDefaultComparerForType(defaultComparerType);
        var value = ((NotEqualRule)rule).Value;
        method = GetValidationMethod<T>(nameof(DefaultValidatorExtensions.NotEqual), prop.Type, prop.Type,
          comparerGenericType);
        methodCall = Expression.Call(null, method!, ruleFor, Expression.Constant(Convert.ChangeType(value, prop.Type)),
          Expression.Constant(defaultComparer));
        break;
      }
      case RuleType.MustBe: {
        var mustMethod = validator.GetType().GetMethodForType(((MustRule)rule).MethodName,
          BindingFlags.NonPublic | BindingFlags.Instance, prop.Type);
        var genericTypeForMustMethod = typeof(Func<,>).MakeGenericType(prop.Type, typeof(bool));
        var arg = Expression.Parameter(prop.Type);
        var mustMethCall = Expression.Call(Expression.Constant(validator), mustMethod!, arg);
        var mustCall = Expression.Lambda(mustMethCall, arg);
        method = GetValidationMethod<T>(nameof(DefaultValidatorExtensions.Must), prop.Type, genericTypeForMustMethod);
        methodCall = Expression.Call(null, method!, ruleFor, mustCall);
        break;
      }
      default:
        throw new NotSupportedException($"Rule {rule.RuleType.ToString()} is not supported");
    }

    var builderOptionsGenericType = typeof(IRuleBuilderOptions<,>).MakeGenericType(typeof(T), prop.Type);
    var builderOptionGenericFunc = typeof(Func<>).MakeGenericType(builderOptionsGenericType);
    var lambdaToCallValidationMethod = Expression.Lambda(builderOptionGenericFunc, methodCall).Compile();
    var validationFuncResult = lambdaToCallValidationMethod.DynamicInvoke();

    if (string.IsNullOrEmpty(rule.Message)) return;

    var messageMethodParams = new[] { builderOptionsGenericType, typeof(string) };
    var withMessageMethod = typeof(DefaultValidatorOptions)
      .GetMethodForType(nameof(DefaultValidatorOptions.WithMessage),
        BindingFlags.Public | BindingFlags.Static, messageMethodParams);

    var theMessage = Expression.Constant(rule.Message);
    var withMessageGenericMethod = withMessageMethod!.MakeGenericMethod(typeof(T), typeof(string));
    builderOptionGenericFunc = typeof(Func<>).MakeGenericType(builderOptionsGenericType);

    var messageMethodCall = Expression.Call(null,
      withMessageGenericMethod!,
      Expression.Constant(validationFuncResult),
      theMessage);

    var lambdaToCallWithMessageMethod = Expression.Lambda(builderOptionGenericFunc, messageMethodCall);

    lambdaToCallWithMessageMethod.Compile().DynamicInvoke();
  }

  private Expression GetMessageMethodCall<T>(AbstractValidator<T> validator, string message) {
    var mustMethod = validator.GetType().GetMethodForType(nameof(DefaultValidatorOptions.WithMessage),
      BindingFlags.NonPublic | BindingFlags.Instance, typeof(string));
    var arg = Expression.Parameter(typeof(string));
    var messageMethodCall = Expression.Call(Expression.Constant(validator), mustMethod!, arg);
    return Expression.Lambda(messageMethodCall, arg);
  }

  private object GetDefaultComparerForType(Type type) => Activator.CreateInstance(type)!;

  private static MethodInfo? GetValidationMethod<TObj>(string methodName, Type propType, params Type[] paramTypes) {
    var methodParamTypes = new List<Type> { typeof(IRuleBuilder<,>) };
    methodParamTypes.AddRange(paramTypes);
    var method = typeof(DefaultValidatorExtensions).GetMethodForType(methodName, methodParamTypes.ToArray())
                 ?? throw new NullReferenceException($"Unable to find method {methodName} in type " +
                                                     $"{nameof(DefaultValidatorExtensions)}");
    return method!.GetGenericArguments().Length == 2
      ? method.MakeGenericMethod(typeof(TObj), propType)
      : method.MakeGenericMethod(typeof(TObj));
  }

  private static ConstantExpression GetRuleFor<T>(AbstractValidator<T> validator, ValidatedProperty prop) {
    var pT = Expression.Parameter(typeof(T));
    var propExp = Expression.PropertyOrField(pT, prop.PropertyName);
    var genericType = typeof(Func<,>).MakeGenericType(typeof(T), prop.Type);
    var lambda = Expression.Lambda(genericType, propExp, pT);

    var ruleFor = validator.GetType().BaseType!.GetMethod("RuleFor")!.MakeGenericMethod(prop.Type);
    var result = ruleFor.Invoke(validator, new object?[] { lambda });
    return Expression.Constant(result);
  }
}