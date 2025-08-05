---
title: Auto and recursive mocks
---

Once a substitute has been created some properties and methods will automatically return non-null values. For example, any properties or methods that return an _interface_, _delegate_, or _purely virtual class_* will automatically return substitutes themselves. This is commonly referred to as _recursive mocking_, and can be useful because you can avoid explicitly creating each substitute, which means less code. Other types, like `String` and `Array`, will default to returning empty values rather than nulls.

_* Note:_ A pure virtual class is defined as one with all its public methods and properties defined as _virtual_ or _abstract_ and with a default, parameterless constructor defined as _public_ or _protected_.

## Recursive mocks

Say we have the following types:

```csharp
public interface INumberParser {
    IEnumerable<int> Parse(string expression);
}
public interface INumberParserFactory {
    INumberParser Create(char delimiter);
}
```

We want to configure our `INumberParserFactory` to create a parser that will return a certain set of `int` for an `expresion`. We could manually create each substitute:

```csharp
var factory = Substitute.For<INumberParserFactory>();
var parser = Substitute.For<INumberParser>();
factory.Create(',').Returns(parser);
parser.Parse("an expression").Returns(new[] {1,2,3});

Assert.That(
    factory.Create(',').Parse("an expression"),
    Is.EqualTo(new[] {1,2,3}));
```

Or we could use the fact that a substitute for type `INumberParser` will automatically be returned for `INumberParserFactory.Create()`:

```csharp
var factory = Substitute.For<INumberParserFactory>();
factory.Create(',').Parse("an expression").Returns(new[] {1,2,3});

Assert.That(
    factory.Create(',').Parse("an expression"),
     Is.EqualTo(new[] {1,2,3}));
```

Each time a recursively-subbed property or method is called with the same arguments it will return the same substitute. If a method is called with different arguments a new substitute will be returned.

<!--
```requiredcode
INumberParserFactory factory;
[SetUp] public void SetUp() { factory = Substitute.For<INumberParserFactory>(); }
```
-->

```csharp
var firstCall = factory.Create(',');
var secondCall = factory.Create(',');
var thirdCallWithDiffArg = factory.Create('x');

Assert.That(firstCall, Is.SameAs(secondCall));
Assert.That(firstCall, Is.Not.SameAs(thirdCallWithDiffArg));
```

_Note:_ Recursive substitutes will not be created for  non-purely virtual classes, as creating and using classes can have potentially unwanted side-effects. You'll therefore need to create and return these explicitly.

### Substitute chains

It is not really an ideal practice, but when required we can also use recursive mocks to make it easier to set up chains of substitutes. For example:

```csharp
public interface IContext {
    IRequest CurrentRequest { get; }
}
public interface IRequest {
    IIdentity Identity { get; }
    IIdentity NewIdentity(string name);
}
public interface IIdentity {
    string Name { get; }
    string[] Roles();
}
```

To get the identity of the `CurrentRequest` to return a certain name, we could manually create substitutes for `IContext`, `IRequest`, and `IIdentity`, and then use `Returns()` to chain these substitutes together. Instead we can use the substitutes automatically created for each property and method:

```csharp
var context = Substitute.For<IContext>();
context.CurrentRequest.Identity.Name.Returns("My pet fish Eric");
Assert.That(
    context.CurrentRequest.Identity.Name,
    Is.EqualTo("My pet fish Eric"));
```

Here `CurrentRequest` is automatically going to return a substitute for `IRequest`, and the `IRequest` substitute will automatically return a substitute for `IIdentity`.

_Note:_ Setting up long chains of substitutes this way is a code smell: we are breaking the [Law of Demeter](https://en.wikipedia.org/wiki/Law_of_Demeter), which says objects should only talk to their immediate neighbours, not reach into their neighbours' neighbours. If you write your tests without recursive mocks this becomes quite obvious as the set up becomes quite complicated, so if you are going to use recursive mocking you'll need to be extra vigilant to avoid this type of coupling.

## Auto values
Properties and methods returning types of `String` or `Array` will automatically get empty, non-null defaults. This can help avoid null reference exceptions in cases where you just need a reference returned but don't care about its specific properties.

```csharp
var identity = Substitute.For<IIdentity>();

Assert.That(identity.Name, Is.EqualTo(String.Empty));
Assert.That(identity.Roles().Length, Is.EqualTo(0));
```

## Customizing auto values

NSubstitute's auto value generation can be customized to suit specific needs. This is particularly useful when you want to override the default behavior for certain types or add support for types that aren't handled by default.

The customization follows the open/closed principle: the system is open for extension but closed for modification. To customize auto values, you can:

1. Create a derived container from `NSubstituteDefaultFactory.DefaultContainer.Customize()`
2. Use the `.Decorate()` method to enhance existing implementations
3. Replace `SubstitutionContext.Current` with your customized context, ideally using Module Initializers

### Example: Custom Task auto values

Here's a complete example showing how to customize auto value generation for `Task` types:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Routing.AutoValues;

namespace YourProject;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    internal static void ConfigureNSubstitute()
    {
        var customizedContainer = NSubstituteDefaultFactory.DefaultContainer.Customize();
        customizedContainer.Decorate<IAutoValueProvidersFactory>(
            (factory, _resolver) => new CustomAutoValueProvidersFactory(factory));

        SubstitutionContext.Current = customizedContainer.Resolve<ISubstitutionContext>();
    }
}

internal class CustomAutoValueProvidersFactory : IAutoValueProvidersFactory
{
    private readonly IAutoValueProvidersFactory _original;

    public CustomAutoValueProvidersFactory(IAutoValueProvidersFactory original)
    {
        _original = original;
    }
    
    public IReadOnlyCollection<IAutoValueProvider> CreateProviders(ISubstituteFactory substituteFactory)
    {
        var originalProviders = _original.CreateProviders(substituteFactory);

        var customTaskProvider = new CustomTaskProvider(originalProviders);

        return new[] { customTaskProvider }.Concat(originalProviders).ToArray();
    }

    private class CustomTaskProvider : IAutoValueProvider
    {
        private readonly IReadOnlyCollection<IAutoValueProvider> _allProviders;

        public CustomTaskProvider(IReadOnlyCollection<IAutoValueProvider> allProviders)
        {
            _allProviders = allProviders;
        }
        
        public bool CanProvideValueFor(Type type) => type == typeof(Task);

        public object GetValue(Type type)
        {
            // You can use _allProviders to recursively resolve inner values if needed
            return Task.FromException(new InvalidOperationException("Custom failed task"));
        }
    }
}
```

### Usage

Once configured, your customization will automatically apply to all substitutes:

```csharp
public interface IService
{
    Task DoWorkAsync();
}

[Test]
public async Task CustomTaskBehavior()
{
    // arrange
    var service = Substitute.For<IService>();

    // act
    var task = service.DoWorkAsync(); // Returns custom failed task

    // assert
    var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () => await task);
    Assert.That(ex.Message, Is.EqualTo("Custom failed task"));
}
```

### Key concepts

- **Module Initializers**: Use the `[ModuleInitializer]` attribute to configure NSubstitute before any substitutes are created. This ensures your customizations are applied globally.
- **Container Customization**: Call `DefaultContainer.Customize()` to create a copy of the default container that you can modify.
- **Decoration Pattern**: Use `.Decorate<T>()` to wrap existing implementations with your custom logic, maintaining the original behavior while adding new functionality.
- **Immutability**: The container follows an immutable pattern - modifications create new instances rather than changing existing ones.

This approach allows you to extend NSubstitute's behavior without modifying its core functionality, making it suitable for scenarios like Unity's `UniTask`, custom async patterns, or domain-specific value generation.