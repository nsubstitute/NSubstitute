---
title: Creating a substitute
layout: post
---

The basic syntax for creating a substitute is:

{% examplecode csharp %}
var substitute = Substitute.For<ISomeInterface>();
{% endexamplecode %}

This is how you'll normally create substitutes for types. Generally this type will be an interface, but you can also substitute classes in cases of emergency.

*Warning:* Substituting for classes can have some nasty side-effects. For starters, NSubstitute can only work with virtual members of the class, so any non-virtual code in the class will actually execute! If you try to substitute for your class that formats your hard drive in the constructor or in a non-virtual property setter then you're asking for trouble. If possible, stick to substituting interfaces.

With the knowledge that we're not going to be substituting for classes, here is how you create a substitute for a class that has constructor arguments:

{% examplecode csharp %}
var someClass = Substitute.For<SomeClassWithCtorArgs>(5, "hello world");
{% endexamplecode %}

For classes that have default constructors the syntax is the same as substituting for interfaces.

## Substituting for multiple interfaces

There are times when you want to substitute for multiple types. The best example of this is when you have code that works with a type, then checks whether it implements <code>IDisposable</code> and disposes of it if it doesn't.

{% examplecode csharp %}
var command = Substitute.For<ICommand, IDisposable>();
var runner = new CommandRunner(command);

runner.RunCommand();

command.Received().Execute();
((IDisposable)command).Received().Dispose();
{% endexamplecode %}

Your substitute can implement several types this way, but remember you can only implement a maximum of one class. You can specify as many interfaces as you like, but only one of these can be a class. The most flexible way of creating substitutes for multiple types is using this overload:

{% examplecode csharp %}
var substitute = Substitute.For(
		new[] { typeof(ICommand), typeof(ISomeInterface), typeof(SomeClassWithCtorArgs) },
		new object[] { 5, "hello world" }
	);
Assert.IsInstanceOf<ICommand>(substitute);
Assert.IsInstanceOf<ISomeInterface>(substitute);
Assert.IsInstanceOf<SomeClassWithCtorArgs>(substitute);
{% endexamplecode %}

{% requiredcode %}
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
{% endrequiredcode %}

## Substituting for delegates

NSubstitute can also substitute for delegate types by using `Substiute.For<T>()`. When substituting for delegate types you will not be able to get the substitute to implement additional interfaces or classes.

{% examplecode csharp %}
var func = Substitute.For<Func<string>>();

func().Returns("hello");
Assert.AreEqual("hello", func());
{% endexamplecode %}





