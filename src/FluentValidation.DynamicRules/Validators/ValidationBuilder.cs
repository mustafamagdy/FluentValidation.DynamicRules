using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using FastExpressionCompiler;
using FluentValidation.DynamicRules.Extensions;
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
    var p = Expression.Parameter(typeof(T));
    var ruleFor = validator.GetRuleFor(p, prop);
    // MethodInfo? method;
    MethodCallExpression methodCall;
    var validatedType = typeof(T);
    var propType = validatedType.GetPropertyType(prop.PropertyName);

    switch (rule.RuleType) {
      case RuleType.NotNull: {
        var method = validatedType.GetNotNullValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.NotEmpty: {
        var method = validatedType.GetNotEmptyValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.Length: {
        var method = validatedType.GetLengthValidator(propType, typeof(int), typeof(int));

        var (min, max) = (LengthRule)rule;
        var arg01 = Expression.Constant(min);
        var arg02 = Expression.Constant(max);

        methodCall = Expression.Call(null, method!, ruleFor, arg01, arg02);

        break;
      }
      case RuleType.NotEqual:
      case RuleType.LessThan:
      case RuleType.LessThanOrEqual:
      case RuleType.GreaterThan:
      case RuleType.GreaterThanOrEqual: {
        methodCall = BuildForValueBasedRules(rule, propType, validatedType, ruleFor, p);
        break;
      }
      case RuleType.MustBe: {
        var predicateMethod = validator.GetType().GetPrivateMethodForType(((MustRule)rule).MethodName, propType);
        var predicateFunc = typeof(Func<,>).MakeGenericType(propType, typeof(bool));

        var method = validatedType.GetPredicateValidator(propType, predicateFunc);

        var instance = Expression.Constant(validator);
        var arg01 = Expression.Parameter(propType);

        var predicateCall = Expression.Call(instance, predicateMethod!, arg01);
        var predicate = Expression.Lambda(predicateCall, arg01);

        methodCall = Expression.Call(null, method!, ruleFor, predicate);
        break;
      }
      default:
        throw new NotSupportedException($"Rule {rule.RuleType:S} is not supported");
    }

    var builderOptionsGenericType = typeof(IRuleBuilderOptions<,>).MakeGenericType(typeof(T), propType);
    var builderOptionGenericFunc = typeof(Func<>).MakeGenericType(builderOptionsGenericType);
    var validationFuncResult = Expression.Lambda(builderOptionGenericFunc, methodCall).CompileFast().DynamicInvoke();

    if (string.IsNullOrEmpty(rule.Message)) return;

    var withMessageMethod = builderOptionsGenericType.GetWithMessageMethod<T>(propType);
    var argResult = Expression.Constant(validationFuncResult);
    var theMessage = Expression.Constant(rule.Message);
    var messageMethodCall = Expression.Call(null, withMessageMethod!, argResult, theMessage);

    var lambdaToCallWithMessageMethod = Expression.Lambda(builderOptionGenericFunc, messageMethodCall);
    lambdaToCallWithMessageMethod.CompileFast().DynamicInvoke();
  }

  private MethodCallExpression BuildForValueBasedRules(PropertyRule rule, Type propType,
    Type validatedType, ConstantExpression ruleFor, ParameterExpression p) {
    var value = ((ValueBasedRules)rule).Value;
    var anotherProp = ((ValueBasedRules)rule).AnotherProp;
    Expression arg01 = null;
    if (value is null && anotherProp is null) {
      throw new ArgumentOutOfRangeException(nameof(value));
    }

    var arg02 = Expression.Constant(propType.GetDefaultComparerForType());

    if (value is null) {
      var theOtherProp = validatedType.GetProperty(anotherProp!, BindingFlags.Instance | BindingFlags.Public
        | BindingFlags.IgnoreCase);
      var memberExpression = Expression.MakeMemberAccess(p, theOtherProp!);
      var method = rule switch {
        NotEqualRule => validatedType.GetNotEqualValidatorWithAnotherProperty(propType),
        LessThanRule => validatedType.GetLessThanValidatorWithAnotherProperty(propType),
        LessThanOrEqualRule => validatedType.GetLessThanOrEqualValidatorWithAnotherProperty(propType),
        GreaterThanRule => validatedType.GetGreaterThanValidatorWithAnotherProperty(propType),
        GreaterThanOrEqualRule => validatedType.GetGreaterThanOrEqualValidatorWithAnotherProperty(propType),
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
      };

      arg01 = Expression.Lambda(memberExpression, p);
      var methodCall = method!.GetParameters().Length == 2
        ? Expression.Call(null, method!, ruleFor, arg01)
        : Expression.Call(null, method!, ruleFor, arg01, arg02);
      return methodCall;
    } else {
      arg01 = Expression.Constant(Convert.ChangeType(value, propType));
      var method = rule switch {
        NotEqualRule => validatedType.GetNotEqualValidator(propType),
        LessThanRule => validatedType.GetLessThanValidator(propType),
        LessThanOrEqualRule => validatedType.GetLessThanOrEqualValidator(propType),
        GreaterThanRule => validatedType.GetGreaterThanValidator(propType),
        GreaterThanOrEqualRule => validatedType.GetGreaterThanOrEqualValidator(propType),
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
      };
      var methodCall = method!.GetParameters().Length == 2
        ? Expression.Call(null, method, ruleFor, arg01)
        : Expression.Call(null, method, ruleFor, arg01, arg02);
      return methodCall;
    }
  }
}