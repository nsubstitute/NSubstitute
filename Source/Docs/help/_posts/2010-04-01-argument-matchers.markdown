---
title: Argument matchers
layout: post
---

Argument matchers can be used when [setting return values](/help/set-return-values) and when [checking received calls](/help/received-calls). They provide a way to _specify_ a call or group of calls, so that a return value can be set for all matching calls, or to check a matching call has been received.

The following type will be used to these examples:

{% examplecode csharp %}
public interface IMovieFinder {
     Movies FindTitle(string title, int limit);
}
{% endexamplecode %}
{% requiredcode %}
public class Movies{}
IMovieFinder finder;
[SetUp] public void SetUp() { finder = Substitute.For<IMovieFinder>(); }
{% endrequiredcode %}

## Ignoring arguments
An argument of type `T` can be ignored using `Arg.Any<T>()`.

{% examplecode csharp %}
var movies = new Movies();
finder.FindTitle(Arg.Any<string>(), 10).Returns(movies);

Assert.That(finder.FindTitle("Any title here", 10), Is.EqualTo(movies));
{% endexamplecode %}

## Matching a specific argument
An argument of type `T` can be matching using `Arg.Is<T>(T value)`.

{% examplecode csharp %}
finder.FindTitle("Toy Story 3", 5);

finder.Received().FindTitle(Arg.Is("Toy Story 3"), 5);
{% endexamplecode %}

This matcher normally isn't required (i.e. we could have just used "Toy Story 3"). It is sometimes necessary however, when NSubstitute can't work out which matcher applies to which argument. In these cases it will throw an `AmbigousArgumentsException` and ask you to specify one or more additional argument matchers.

<!-- show EXAMPLE -->

## Conditionally matching an argument
An argument of type `T` can be conditionally matched using `Arg.Is<T>(Predicate<T> condition)`.


