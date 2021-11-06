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
        method = GetValidationMethod<T>("NotEmpty", prop.Type);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.Length: {
        method = GetValidationMethod<T>("Length", prop.Type, typeof(int), typeof(int));
        methodCall = Expression.Call(null, method!, ruleFor, Expression.Constant(((LengthRule)rule).Min),
          Expression.Constant(((LengthRule)rule).Max));
        break;
      }
      case RuleType.NotEqual: {
        var comparerGenericType = typeof(IEqualityComparer<>).MakeGenericType(prop.Type);
        var defaultComparerType = typeof(DefaultEqualityComparer<>).MakeGenericType(prop.Type);
        var defaultComparer = Activator.CreateInstance(defaultComparerType);

        method = GetValidationMethod<T>("NotEqual", prop.Type, prop.Type, comparerGenericType);
        methodCall = Expression.Call(null,
          method!,
          ruleFor,
          Expression.Constant(Convert.ChangeType(((NotEqualRule)rule).Value, prop.Type)),
          Expression.Constant(defaultComparer));
        break;
      }
      case RuleType.MustBe: {
        var mustMethod = validator.GetType().GetStaticMethodForType(((MustRule)rule).MethodName,
          BindingFlags.NonPublic | BindingFlags.Instance, prop.Type);
        var genericTypeForMustMethod = typeof(Func<,>).MakeGenericType(prop.Type, typeof(bool));
        var arg = Expression.Parameter(prop.Type);
        var mustMethCall = Expression.Call(Expression.Constant(validator), mustMethod!, arg);
        var mustCall = Expression.Lambda(mustMethCall, arg);
        method = GetValidationMethod<T>("Must", prop.Type, genericTypeForMustMethod);
        methodCall = Expression.Call(null,
          method!,
          ruleFor,
          mustCall);
        break;
      }
      default:
        throw new NotSupportedException($"Rule {rule.RuleType.ToString()} is not supported");
    }

    var builderGenericType = typeof(IRuleBuilderOptions<,>).MakeGenericType(typeof(T), prop.Type);
    var genericLambda = typeof(Func<>).MakeGenericType(builderGenericType);
    var lambda = Expression.Lambda(genericLambda, methodCall).Compile();
    lambda.DynamicInvoke();
  }

  private static MethodInfo? GetValidationMethod<TObj>(string methodName, Type propType, params Type[] paramTypes) {
    var methodParamTypes = new List<Type> { typeof(IRuleBuilder<,>) };
    methodParamTypes.AddRange(paramTypes);
    var method = typeof(DefaultValidatorExtensions).GetStaticMethodForType(methodName, methodParamTypes.ToArray())
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
    var result = ruleFor.Invoke(validator, new[] { lambda });
    return Expression.Constant(result);
  }
}