---
title: Creating a substitute
layout: post
---

The basic syntax for creating a substitute is:

{% highlight csharp %}
var substitute = Substitute.For<ISomeInterface>();
{% endhighlight %}

This is how you'll normally create substitutes for types. Generally this type will be an interface, but you can also substitute classes in cases of emergency.

*Warning:* Substituting for classes can have some nasty side-effects. For starters, NSubstitute can only work with virtual members of the class, so any non-virtual code in the class will actually execute! If you try to substitute for your class that formats your hard drive in the constructor or in a non-virtual property setter then you're asking for trouble. If possible, stick to substituting interfaces.

With the knowledge that we're not going to be substituting for classes, here is how you create a substitute for a class that has constructor arguments:

{% highlight csharp %}
var someClass = Substitute.For<SomeClassWithCtorArgs>(5, "hello world");
{% endhighlight %}

For classes that have default constructors the syntax is the same as substituting for interfaces.

## Substituting for multiple interfaces

There are times when you want to substitute for multiple types. The best example of this is when you have code that works with a type, then checks whether it implements <code>IDisposable</code> and disposes of it if it doesn't.

{% highlight csharp %}
var command = Substitute.For<ICommand, IDisposable>();
var runner = new CommandRunner(command);

runner.RunCommand();

command.Received().Execute();
((IDisposable)command).Received().Dispose();
{% endhighlight %}

You're substitute can implement several types this way, but remember you can only implement a maximum of one class. You can specify as many interfaces as you like, but only one of these can be a class. The most flexible way of creating substitutes for multiple types is using this overload:

{% highlight csharp %}
var substitute = Substitute.For(
		new[] { typeof(ICommand), typeof(ISomeInterface), typeof(SomeClassWithCtorArgs) },
		new object[] { 5, "hello world" }
	);
Assert.IsInstanceOf<ICommand>(substitute);
Assert.IsInstanceOf<ISomeInterface>(substitute);
Assert.IsInstanceOf<SomeClassWithCtorArgs>(substitute);
{% endhighlight %}






