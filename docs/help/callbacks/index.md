---
title: Callbacks, void calls and When..Do
---

Sometimes it is useful to execute some arbitrary code whenever a particular call is made. We have already seen an example of this when [passing functions to `Returns()`](/help/return-from-function/#callbacks).

<!--
```requiredcode
public interface ICalculator {
    int Add(int a, int b);
    string Mode { get; set; }
}
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
```
-->

```csharp
var counter = 0;
calculator
    .Add(default, default)
    .ReturnsForAnyArgs(x => 0)
    .AndDoes(x => counter++);

calculator.Add(7, 3);
calculator.Add(2, 2);
calculator.Add(11, -3);
Assert.That(counter, Is.EqualTo(3));
```

The [Return from a function](/help/return-from-function) topic has more information on the arguments passed to the callback.

## Callbacks for `void` calls

`Returns()` can be used to get callbacks for members that return a value, but for `void` members we need a different technique, because we can't call a method on a `void` return. For these cases we can use the `When..Do` syntax.

## When called, do this

`When..Do` uses two calls to configure our callback. First, `When()` is called on the substitute and passed a function. The argument to the function is the substitute itself, and we can call the member we are interested in here, even if it returns `void`. We then call `Do()` and pass in our callback that will be executed when the substitute's member is called.

```csharp
public interface IFoo {
    void SayHello(string to);
}
[Test]
public void SayHello() {
    var counter = 0;
    var foo = Substitute.For<IFoo>();
    foo.When(x => x.SayHello("World"))
        .Do(x => counter++);

    foo.SayHello("World");
    foo.SayHello("World");
    Assert.That(counter, Is.EqualTo(2));
}
```

The argument passed to the `Do()` method is the same call information passed to the [`Returns()` callback](/help/return-from-function), which gives us access to the arguments used for the call.

Note that we can also use `When..Do` syntax for non-void members, but generally the `Returns()` syntax is preferred for brevity and clarity. You may still find it useful for non-voids when you want to execute a function without changing a previous return value.

```csharp
var counter = 0;
calculator.Add(1, 2).Returns(3);
calculator
    .When(x => x.Add(Arg.Any<int>(), Arg.Any<int>()))
    .Do(x => counter++);

var result = calculator.Add(1, 2);
Assert.That(result, Is.EqualTo(3));
Assert.That(counter, Is.EqualTo(1));
```

## Per argument callbacks

In cases where we only need callbacks for a particular argument we may be able to use [per argument callbacks like `Arg.Do()` and `Arg.Invoke()`](/help/actions-with-arguments) instead of `When..Do`.

Argument callbacks give us slightly more concise code in a style that is more in keeping with the rest of the NSubstitute API. See [Actions with arguments](/help/actions-with-arguments) for more information and examples.


## Callback builder for more complex callbacks

The `Callback` builder lets us create more complex `Do()` scenarios.  We can use `Callback.First()` followed by `Then()`, `ThenThrow()` and `ThenKeepDoing()` to build chains of callbacks. We can also use `Always()` and `AlwaysThrow()` to specify callbacks called every time. Note that a callback set by an `Always()` method will be called even if other callbacks will throw an exception.

<!--
```requiredcode
public interface ISomething { void Something(); }
```
-->

```csharp
var sub = Substitute.For<ISomething>();

var calls = new List<string>();
var counter = 0;

sub
  .When(x => x.Something())
  .Do(
    Callback.First(x => calls.Add("1"))
        .Then(x => calls.Add("2"))
        .Then(x => calls.Add("3"))
        .ThenKeepDoing(x => calls.Add("+"))
        .AndAlways(x => counter++)
  );

for (int i = 0; i < 5; i++)
{
  sub.Something();
}
Assert.That(String.Concat(calls), Is.EqualTo("123++"));
Assert.That(counter, Is.EqualTo(5));
```