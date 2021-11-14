using System;
using System.Linq;
using System.Xml.Linq;
using FluentValidation.DynamicRules.Rules;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules {
  public class RuleParser<T> : IRuleParser<T> {
    public ValidationBuilder<T> Parse(string text) {
      var nodes = XElement.Parse(text);
      var ruleSet = (from propNodes in nodes.Elements("rule-for")
        select new {
          Prop = propNodes.Attribute("prop")!.Value.ToString(),
          PropType = propNodes.Attribute("type")?.Value ?? typeof(string).ToString(),
          Rules = propNodes.Elements()
        }).ToArray();

      var validatedProperties = ruleSet.Select(a => {
        var validatedProperty = new ValidatedProperty(a.Prop, a.Rules.Select(ParseRule));
        return validatedProperty;
      });
      return new ValidationBuilder<T>(validatedProperties);
    }

    private PropertyRule ParseRule(XElement node) {
      var message = node.Attribute("message")?.Value ?? "";
      switch (node.Name.LocalName) {
        case "not-null": {
          return new NotNullRule(message);
        }
        case "null": {
          return new NullRule(message);
        }
        case "not-empty": {
          return new NotEmptyRule(message);
        }
        case "empty": {
          return new EmptyRule(message);
        }
        case "email": {
          return new EmailAddressRule(message);
        }
        case "credit-card": {
          return new CreditCardRule(message);
        }
        case "string-len": {
          if (node.Attribute("value") == null && node.Attribute("min") == null && node.Attribute("max") == null)
            throw new ArgumentException(
              "No value provided for length, either value, or min and max should be provided.");

          var fixedLength = node.Attribute("value");
          int min, max;
          if (fixedLength != null) {
            min = max = Convert.ToInt32(fixedLength.Value);
          } else {
            min = Convert.ToInt32(node.Attribute("min")!.Value);
            max = Convert.ToInt32(node.Attribute("max")!.Value);
          }

          return new LengthRule(message, min, max);
        }
        case "must-be": {
          var methodName = node.Attribute("call")?.Value;
          var methodNameWithParentObject = node.Attribute("with-parent")?.Value;
          var methodNameWithParentObjectAndContext = node.Attribute("with-context")?.Value;
          return new MustRule(message, methodName ?? "",
            methodNameWithParentObject ?? "",
            methodNameWithParentObjectAndContext ?? "");
        }
        case "exclusive-between": {
          var min = Convert.ToInt32(node.Attribute("min")!.Value);
          var max = Convert.ToInt32(node.Attribute("max")!.Value);
          return new ExclusiveBetweenRule(message, min, max);
        }
        case "inclusive-between": {
          var min = Convert.ToInt32(node.Attribute("min")!.Value);
          var max = Convert.ToInt32(node.Attribute("max")!.Value);
          return new InclusiveBetweenRule(message, min, max);
        }
        default: {
          var value = node.Attribute("value")?.Value;
          var anotherProp = node.Attribute("prop")?.Value;
          return node.Name.LocalName switch {
            "not-equal" => new NotEqualRule(message, value, anotherProp),
            "less-than" => new LessThanRule(message, value, anotherProp),
            "less-than-equal" => new LessThanOrEqualRule(message, value, anotherProp),
            "greater-than" => new GreaterThanRule(message, value, anotherProp),
            "greater-than-equal" => new GreaterThanOrEqualRule(message, value, anotherProp),
            "equal" => new EqualRule(message, value, anotherProp),
            "min-len" => new MinLengthRule(message, value),
            "max-len" => new MaxLengthRule(message, value),
            _ => throw new NotSupportedException($"{node.Name} is not supported.")
          };
        }
      }
    }
  }
}