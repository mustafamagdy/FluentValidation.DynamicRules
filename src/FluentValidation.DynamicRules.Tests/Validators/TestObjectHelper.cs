using System.Collections.Generic;
using Bogus;

namespace FluentValidation.DynamicRules.Tests.Validators;

public class TestObjectHelper {
  private static readonly Faker Faker = new();

  public static MainObject New => new() {
    Text1 = Faker.Random.Word(),
    Text2 = Faker.Random.Word(),
    Text3 = Faker.Random.Word(),
  };

  public static MainObject NewWithSubItem => new() {
    Text1 = Faker.Random.Word(),
    Text2 = Faker.Random.Word(),
    Text3 = Faker.Random.Word(),
    SubItems = new List<SubObject>() {
      new() { Int1 = 1 }
    }
  };
}