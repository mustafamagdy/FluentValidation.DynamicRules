using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using FastExpressionCompiler;
using FluentValidation.DynamicRules.Extensions;
using FluentValidation.DynamicRules.Rules;
using FluentValidation.Validators;

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
      case RuleType.Null: {
        var method = validatedType.GetNullValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.NotEmpty: {
        var method = validatedType.GetNotEmptyValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.Empty: {
        var method = validatedType.GetEmptyValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.EmailAddress: {
        var method = validatedType.EmailAddressValidator(propType);
        var arg01 = Expression.Constant(EmailValidationMode.AspNetCoreCompatible);
        methodCall = Expression.Call(null, method!, ruleFor, arg01);
        break;
      }
      case RuleType.CreditCard: {
        var method = validatedType.GetCreditCardValidator(propType);
        methodCall = Expression.Call(null, method!, ruleFor);
        break;
      }
      case RuleType.Length: {
        var method = validatedType.GetLengthValidator(propType);

        var (min, max) = (LengthRule)rule;
        var arg01 = Expression.Constant(min);
        var arg02 = Expression.Constant(max);

        methodCall = Expression.Call(null, method!, ruleFor, arg01, arg02);

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
      case RuleType.Equal:
      case RuleType.NotEqual:
      case RuleType.LessThan:
      case RuleType.LessThanOrEqual:
      case RuleType.GreaterThan:
      case RuleType.GreaterThanOrEqual:
      case RuleType.MinLength:
      case RuleType.MaxLength: {
        methodCall = BuildForValueBasedRules(rule, propType, validatedType, ruleFor, p);
        break;
      }
      default:
        throw new NotSupportedException($"Rule {rule.RuleType:G} is not supported");
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
    Expression? arg01 = null;
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
        EqualRule => validatedType.GetEqualValidatorWithAnotherProperty(propType),
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
      };

      arg01 = Expression.Lambda(memberExpression, p);
      var methodCall = method!.GetParameters().Length == 2
        ? Expression.Call(null, method!, ruleFor, arg01)
        : Expression.Call(null, method!, ruleFor, arg01, arg02);
      return methodCall;
    } else {
      var method = rule switch {
        NotEqualRule => validatedType.GetNotEqualValidator(propType),
        LessThanRule => validatedType.GetLessThanValidator(propType),
        LessThanOrEqualRule => validatedType.GetLessThanOrEqualValidator(propType),
        GreaterThanRule => validatedType.GetGreaterThanValidator(propType),
        GreaterThanOrEqualRule => validatedType.GetGreaterThanOrEqualValidator(propType),
        EqualRule => validatedType.GetEqualValidator(propType),
        MinLengthRule => validatedType.GetMinLengthValidator(propType),
        MaxLengthRule => validatedType.GetMaxLengthValidator(propType),
        _ => throw new ArgumentOutOfRangeException(nameof(rule), rule, null)
      };
      var @params = method!.GetParameters();
      MethodCallExpression methodCall;
      switch (@params.Length) {
        case 2: {
          var argType = method.GetParameters()[1].ParameterType;
          arg01 = Expression.Constant(Convert.ChangeType(value, argType));
          methodCall = Expression.Call(null, method, ruleFor, arg01);
          break;
        }
        case 3:
          arg01 = Expression.Constant(Convert.ChangeType(value, propType));
          methodCall = Expression.Call(null, method, ruleFor, arg01, arg02);
          break;
        default:
          throw new NotSupportedException($"Method {method.Name} has unsupported number of parameters");
      }

      return methodCall;
    }
  }
}