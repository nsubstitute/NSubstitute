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