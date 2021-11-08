using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using FluentValidation.DynamicRules.Rules;
using FluentValidation.DynamicRules.Validators;

namespace FluentValidation.DynamicRules {
  public class RuleParser {
    public ValidationBuilder Parse(string xmlRules) {
      var nodes = XElement.Parse(xmlRules);
      var ruleSet = (from propNodes in nodes.Elements("rule-for")
        select new {
          Prop = propNodes.Attribute("prop")!.Value.ToString(),
          PropType = propNodes.Attribute("type")?.Value ?? typeof(string).ToString(),
          Rules = propNodes.Elements()
        }).ToArray();

      var validatedProperties = ruleSet.Select(a => {
        var propType = Type.GetType(a.PropType);
        var validatedProperty = new ValidatedProperty(a.Prop, propType!, a.Rules.Select(ParseRule));
        return validatedProperty;
      });
      return  new ValidationBuilder(validatedProperties);
    }

    private PropertyRule ParseRule(XElement node) {
      var message = node.Attribute("message")?.Value ?? "";
      switch (node.Name.LocalName) {
        case "not-empty": {
          return new NotEmptyRule(message);
        }
        case "string-len": {
          if (node.Attribute("value") == null && node.Attribute("min") == null && node.Attribute("max") == null)
            throw new ArgumentException("No value provided for length, either value, or min and max should be provided.");

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
        case "not-equal": {
          var value = node.Attribute("value")!.Value;
          var when = node.Attribute("when-prop")?.Value;
          return new NotEqualRule(message, value, when);
        }
        case "must-be": {
          var methodName = node.Attribute("call")!.Value;
          return new MustRule(message, methodName);
        }
        default:
          throw new NotSupportedException($"{node.Name} is not supported.");
      }
    }
  }
}