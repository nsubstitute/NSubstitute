---
title: Creating a substitute
---

The basic syntax for creating a substitute is:

```csharp
var substitute = Substitute.For<ISomeInterface>();
```

This is how you'll normally create substitutes for types. Generally this type will be an interface, but you can also substitute classes in cases of emergency.

## Substituting (infrequently and carefully) for classes

⚠️ **Warning:** Substituting for classes can have some nasty side-effects!

For starters, **NSubstitute can only work with *virtual* members of the class** that are overridable in the test assembly, so any non-virtual code in the class will actually execute! If you try to substitute for a class that formats your hard drive in the constructor or in a non-virtual property setter then you're asking for trouble. By overridable we mean `public virtual`, `protected virtual`, `protected internal virtual`, or `internal virtual` with `InternalsVisibleTo` attribute applied (although to configure or assert on calls members will also need to be callable from the test assembly, so `public virtual` or `internal virtual` with `InternalsVisibleTo`). See [How NSubstitute works](/help/how-nsub-works/) for more information.

It also means features like `Received()`, `Returns()`, `Arg.Is()`, `Arg.Any()` and `When()..Do()` **will not work with these non-overridable members**. For example: `subClass.Received().NonVirtualCall()` will not actually run an assertion (it will always pass, even if there are no calls to `NonVirtualCall()`), and can even cause confusing problems with later tests. These features will work correctly with virtual members of the class, but we have to be careful to avoid the non-virtual ones.

For these reasons we strongly recommend using [NSubstitute.Analyzers](/help/nsubstitute-analysers/) to detect these cases, and sticking to substituting for interfaces as much as possible. (Interfaces are always safe to substitute and do not suffer from any of the limitations that class substitutes do.)

With the knowledge that we're not going to be substituting for classes, here is how you create a substitute for a class that has constructor arguments:

```csharp
var someClass = Substitute.For<SomeClassWithCtorArgs>(5, "hello world");
```

For classes that have default constructors the syntax is the same as substituting for interfaces.

## Substituting for multiple interfaces

There are times when you want to substitute for multiple types. The best example of this is when you have code that works with a type, then checks whether it implements <code>IDisposable</code> and disposes of it if it doesn't.

```csharp
var command = Substitute.For<ICommand, IDisposable>();
var runner = new CommandRunner(command);

runner.RunCommand();

command.Received().Execute();
((IDisposable)command).Received().Dispose();
```

Your substitute can implement several types this way, but remember you can only implement a maximum of one class. You can specify as many interfaces as you like, but only one of these can be a class. The most flexible way of creating substitutes for multiple types is using this overload:

```csharp
var substitute = Substitute.For(
    new[] { typeof(ICommand), typeof(ISomeInterface), typeof(SomeClassWithCtorArgs) },
    new object[] { 5, "hello world" }
);
Assert.That(substitute, Is.InstanceOf<ICommand>());
Assert.That(substitute, Is.InstanceOf<ISomeInterface>());
Assert.That(substitute, Is.InstanceOf<SomeClassWithCtorArgs>());
```

<!--
```requiredcode
public interface ISomeInterface { }
public abstract class SomeClassWithCtorArgs
{
    protected SomeClassWithCtorArgs(int anInt, string aString) { }
}

public interface ICommand
{
    void Execute();
}

public class CommandRunner
{
    private readonly ICommand _command;
    public CommandRunner(ICommand command)
    {
        _command = command;
    }

    public void RunCommand()
    {
        _command.Execute();
        if (_command is IDisposable) ((IDisposable)_command).Dispose();
    }
}
```
-->

## Substituting for delegates

NSubstitute can also substitute for delegate types by using `Substiute.For<T>()`. When substituting for delegate types you will not be able to get the substitute to implement additional interfaces or classes.

```csharp
var func = Substitute.For<Func<string>>();

func().Returns("hello");
Assert.That(func(), Is.EqualTo("hello"));
```

## Partial substitutes and test spies

When required we can also create substitutes that run real code by default, letting us replace [specific parts of a class with substitute behaviour](/help/partial-subs/).