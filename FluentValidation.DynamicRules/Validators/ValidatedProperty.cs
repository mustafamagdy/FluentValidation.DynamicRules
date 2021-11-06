using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.DynamicRules.Rules;

namespace FluentValidation.DynamicRules.Validators;

public sealed class ValidatedProperty {
  public ValidatedProperty(string propertyName, Type type, IEnumerable<PropertyRule> rules) {
    PropertyName = propertyName;
    Type = type;
    Rules = rules.ToArray();
  }

  public string PropertyName { get; }
  public Type Type { get; }
  public PropertyRule[] Rules { get; }
}