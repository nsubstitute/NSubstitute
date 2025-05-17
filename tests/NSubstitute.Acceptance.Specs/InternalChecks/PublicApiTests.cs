using NUnit.Framework;
using System.Reflection;

namespace NSubstitute.Acceptance.Specs.InternalChecks;

[TestFixture]
public class PublicApiTests
{
    private static readonly Assembly Assembly = typeof(Substitute).Assembly;

    [Test]
    [TestCaseSource(nameof(AllPublicProperties))]
    public void Property_Should_Not_Use_Internal_Type(Type type, PropertyInfo property)
    {
        var propertyType = property.PropertyType;

        // Act & Assert
        Assert.That(propertyType.Namespace, Is.Not.Contains(".Internal"), $"Property '{type.FullName}.{property.Name}' uses internal type '{propertyType.FullName}'.");

    }
    [Test]
    [TestCaseSource(nameof(AllPublicMethods))]
    public void Method_Should_Not_Use_Internal_Types(Type type, MethodInfo method)
    {
        // Arrange
        var returnType = method.ReturnType;
        var parameters = method.GetParameters();

        // Act & Assert

        // Check return type
        Assert.That(returnType.Namespace, Is.Not.Contains(".Internal"), $"Method '{type.FullName}.{method.Name}' uses internal return type '{returnType.FullName}'.");

        // Check parameter types
        foreach (var param in parameters)
        {
            var paramType = param.ParameterType;
            Assert.That(paramType.Namespace, Is.Not.Contains(".Internal"), $"Method '{type.FullName}.{method.Name}' parameter '{param.Name}' uses internal type '{paramType.FullName}'.");
        }
    }

    public static IEnumerable<TestCaseData> AllPublicProperties()
    {
        foreach (var type in GetPublicTypesThatAreNotInInternalNamespace())
        {
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                yield return new TestCaseData(type, prop)
                    .SetName($"Property {type.Name}.{prop.Name} ShouldNotUseInternalTypes");
            }
        }
    }

    public static IEnumerable<TestCaseData> AllPublicMethods()
    {
        foreach (var type in GetPublicTypesThatAreNotInInternalNamespace())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (method.IsSpecialName) // Exclude property/events accessors
                    continue;

                yield return new TestCaseData(type, method)
                    .SetName($"Method {type.Name}.{method.Name} ShouldNotUseInternalTypes");
            }
        }
    }

    private static IEnumerable<Type> GetPublicTypesThatAreNotInInternalNamespace()
    {
        return Assembly.GetExportedTypes().Where(t => t.IsPublic && !t.Namespace.Contains(".Internal"));
    }


}
