---
title: Return for all calls of a type
---

We can return a specific value for all calls to a substitute using `sub.ReturnsForAll<T>(T value)`. This will cause `sub` to return `value` for all calls that return something of type `T` and are [not already stubbed](#returns-vs-returnsforall).

**Note: we need `using NSubstitute.Extensions` to import the `.ReturnsForAll<T>()` extension method.**

The type must match exactly: `.ReturnsForAll<Cat>(cat)` will not set a return value for a call that returns `Animal`, even if `Cat` inherits from `Animal`. To return for the super-type, use `.ReturnsForAll<Animal>(cat)`. (If you'd like a change in this behaviour, please [let us know](https://github.com/nsubstitute/NSubstitute/issues)).

There is also an overload that takes a `Func<CallInfo,T>` so the value to return will be calculated each time.

## Fluent interface example

One example of where this can be useful is a builder-style interface where each call returns a reference to itself.

<!--
```requiredcode
public interface IWidgetContainer {}
```
-->

```csharp
// using NSubstitute.Extensions;

public interface IWidgetBuilder {
  IWidgetBuilder Quantity(int i);
  IWidgetBuilder AddLineItem(string s);
  IWidgetContainer GetWidgets();
}

public class ProductionLine {
  IWidgetBuilder builder;
  public ProductionLine(IWidgetBuilder builder) {
    this.builder = builder;
  }

  public IWidgetContainer Run() {
    return builder
              .Quantity(2)
              .AddLineItem("Thingoe")
              .AddLineItem("Other thingoe")
              .GetWidgets();
  }
}

[Test]
public void ShouldReturnWidgetsFromBuilder() {
  var builder = Substitute.For<IWidgetBuilder>();
  builder.ReturnsForAll<IWidgetBuilder>(builder);
  var line = new ProductionLine(builder);

  var result = line.Run();

  Assert.That(result, Is.EqualTo(builder.GetWidgets()));
}
```

In this test `builder` will return a reference to itself whenever a call returns a value of type `IWidgetBuilder`, so the chained calls will all work on the same `builder` instance.

## Returns vs. ReturnsForAll

Calls will only use `.ReturnsForAll` values when NSubstitute can't find an explicitly stubbed value for the call. So if we set `sub.GetWidget().Returns(widget)`, that will take precendence over any values stubbed by `sub.ReturnsForAll<Widget>(otherWidget)`.

<!--
```requiredcode
public class Widget {}
public interface IWidgetExample {
  Widget GetWidget(int i);
}
```
-->

```csharp
[Test]
public void ReturnsTakesPrecedence() {
  var widget = new Widget();
  var otherWidget = new Widget();
  var sub = Substitute.For<IWidgetExample>();
  sub.GetWidget(1).Returns(widget);
  sub.ReturnsForAll<Widget>(otherWidget);

  Assert.That(sub.GetWidget(1), Is.SameAs(widget));
  Assert.That(sub.GetWidget(42), Is.SameAs(otherWidget));
}
```