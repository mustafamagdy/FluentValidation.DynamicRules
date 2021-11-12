using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.DynamicRules.Rules;

namespace FluentValidation.DynamicRules.Validators;

public sealed class ValidatedProperty {
  public ValidatedProperty(string propertyName, IEnumerable<PropertyRule> rules) {
    PropertyName = propertyName;
    Rules = rules.ToArray();
  }

  public string PropertyName { get; }
  public PropertyRule[] Rules { get; }
}