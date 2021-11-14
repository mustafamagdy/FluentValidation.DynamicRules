using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using FastExpressionCompiler;
using FluentValidation.DynamicRules.Extensions;
using FluentValidation.DynamicRules.Rules;
using FluentValidation.Validators;

namespace FluentValidation.DynamicRules.Validators;

public class ValidationBuilder<T> {
  private readonly IEnumerable<ValidatedProperty> _properties;
  public ValidationBuilder(IEnumerable<ValidatedProperty> properties) { _properties = properties; }

  public void BuildFor(AbstractValidator<T> validator) {
    _properties.ForEach(prop => BuildRulesForProperty(prop, validator));
  }

  private void BuildRulesForProperty(ValidatedProperty prop, AbstractValidator<T> validator) {
    prop.Rules.ForEach(rule => BuildRule(rule, prop, validator));
  }

  private void BuildRule(PropertyRule rule, ValidatedProperty prop, AbstractValidator<T> validator) {
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

        var (min, max) = (RangeBasedRule)rule;
        var arg01 = Expression.Constant(min);
        var arg02 = Expression.Constant(max);

        methodCall = Expression.Call(null, method!, ruleFor, arg01, arg02);
        break;
      }
      case RuleType.ExclusiveBetween: {
        var method = validatedType.GetExclusiveBetweenValidator(propType);

        var (min, max) = (RangeBasedRule)rule;
        var arg01 = Expression.Constant(min);
        var arg02 = Expression.Constant(max);

        methodCall = Expression.Call(null, method!, ruleFor, arg01, arg02);
        break;
      }
      case RuleType.InclusiveBetween: {
        var method = validatedType.GetInclusiveBetweenValidator(propType);

        var (min, max) = (RangeBasedRule)rule;
        var arg01 = Expression.Constant(min, typeof(int));
        var arg02 = Expression.Constant(max);

        methodCall = Expression.Call(null, method!, ruleFor, arg01, arg02);
        break;
      }

      case RuleType.MustBe: {
        var (methodName, methodWithParent, methodWithContext) = ((MustRule)rule);
        MethodInfo? predicateMethod = null;
        Type? predicateFunc = null;
        LambdaExpression? predicate = null;
        MethodInfo? method = null;

        var instance = Expression.Constant(validator);

        if (!string.IsNullOrEmpty(methodName)) {
          predicateMethod = validator.GetType().GetPrivateMethodForType(methodName, propType);
          predicateFunc = typeof(Func<,>).MakeGenericType(propType, typeof(bool));

          method = validatedType.GetPredicateValidator(propType, predicateFunc);
          var arg01 = Expression.Parameter(propType);
          var predicateCall = Expression.Call(instance, predicateMethod!, arg01);
          predicate = Expression.Lambda(predicateCall, arg01);
        } else if (!string.IsNullOrEmpty(methodWithParent)) {
          predicateMethod = validator.GetType().GetPrivateMethodForType(methodWithParent, typeof(T), propType);
          predicateFunc = typeof(Func<,,>).MakeGenericType(typeof(T), propType, typeof(bool));

          method = validatedType.GetPredicateValidator(propType, predicateFunc);
          var arg01 = Expression.Parameter(typeof(T));
          var arg02 = Expression.Parameter(propType);
          var predicateCall = Expression.Call(instance, predicateMethod!, arg01, arg02);
          predicate = Expression.Lambda(predicateCall, arg01, arg02);
        } else if (!string.IsNullOrEmpty(methodWithContext)) {
          predicateMethod = validator.GetType().GetPrivateMethodForType(methodWithContext, typeof(T), propType,
            typeof(ValidationContext<T>));
          predicateFunc = typeof(Func<,,,>).MakeGenericType(typeof(T), propType,
            typeof(ValidationContext<T>), typeof(bool));

          method = validatedType.GetPredicateValidator(propType, predicateFunc);
          var arg01 = Expression.Parameter(typeof(T));
          var arg02 = Expression.Parameter(propType);
          var arg03 = Expression.Parameter(typeof(ValidationContext<T>));
          var predicateCall = Expression.Call(instance, predicateMethod!, arg01, arg02, arg03);
          predicate = Expression.Lambda(predicateCall, arg01, arg02, arg03);
        }


        methodCall = Expression.Call(null, method!, ruleFor, predicate!);
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

  private static MethodCallExpression BuildForValueBasedRules(PropertyRule rule, Type propType,
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