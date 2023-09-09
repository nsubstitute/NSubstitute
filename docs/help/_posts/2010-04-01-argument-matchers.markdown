---
title: Argument matchers
layout: post
---

Argument matchers can be used when [setting return values](/help/return-for-args) and when [checking received calls](/help/received-calls). They provide a way to _specify_ a call or group of calls, so that a return value can be set for all matching calls, or to check a matching call has been received.

The argument matchers syntax shown here depends on having C# 7.0 or later. If you are stuck on an earlier version (getting an error such as `CS7085: By-reference return type 'ref T' is not supported` while trying to use them) please use [compatibility argument matchers](/help/compat-args) instead. 

⚠️ **Note:** Argument matchers should only be used when setting return values or checking received calls. Using `Arg.Is` or `Arg.Any` without a call to `Returns(...)` or `Received()` can cause your tests to behave in unexpected ways. See [How NOT to use argument matchers](#how_not_to_use_argument_matchers) for more information.

<!--
```requiredcode
public interface ICalculator {
    int Add(int a, int b);
    int Subtract(int a, int b);
    void StoreMemory(int slot, int value);
    bool LoadMemory(int slot, out int value);
}
public interface IFormatter {
  string Format(object o);
}
public interface IWidgetFactory {
  string Make(WidgetInfo info);
  string MakeDefaultWidget();
}
public class WidgetInfo {
  public string Name { get; set; }
  public int Quantity { get; set; }
}
public class Sprocket {
  IWidgetFactory wf;
  public Sprocket(IWidgetFactory wf) { this.wf = wf; }
  public void StartWithWidget(WidgetInfo info) { wf.Make(info); }
}
ICalculator calculator;
IFormatter formatter;
string TestWidget = "test widget";
[SetUp] public void SetUp() { 
    calculator = Substitute.For<ICalculator>(); 
    formatter = Substitute.For<IFormatter>();
}
```
-->

## Ignoring arguments
An argument of type `T` can be ignored using `Arg.Any<T>()`.

```csharp
calculator.Add(Arg.Any<int>(), 5).Returns(7);

Assert.AreEqual(7, calculator.Add(42, 5));
Assert.AreEqual(7, calculator.Add(123, 5));
Assert.AreNotEqual(7, calculator.Add(1, 7));
```

In this example we return `7` when adding any number to `5`. We use `Arg.Any<int>()` to tell NSubstitute to ignore the first argument.

We can also use this to match any argument of a specific sub-type.

```csharp
formatter.Format(new object());
formatter.Format("some string");

formatter.Received().Format(Arg.Any<object>());
formatter.Received().Format(Arg.Any<string>());
formatter.DidNotReceive().Format(Arg.Any<int>());
```

## Conditionally matching an argument
An argument of type `T` can be conditionally matched using `Arg.Is<T>(Predicate<T> condition)`.

```csharp
calculator.Add(1, -10);

//Received call with first arg 1 and second arg less than 0:
calculator.Received().Add(1, Arg.Is<int>(x => x < 0));
//Received call with first arg 1 and second arg of -2, -5, or -10:
calculator
    .Received()
    .Add(1, Arg.Is<int>(x => new[] {-2,-5,-10}.Contains(x)));
//Did not receive call with first arg greater than 10:
calculator.DidNotReceive().Add(Arg.Is<int>(x => x > 10), -10);
```

If the condition throws an exception for an argument, then it will be assumed that the argument does not match. The exception itself will be swallowed.

```csharp
formatter.Format(Arg.Is<string>(x => x.Length <= 10)).Returns("matched");

Assert.AreEqual("matched", formatter.Format("short"));
Assert.AreNotEqual("matched", formatter.Format("not matched, too long"));
// Will not match because trying to access .Length on null will throw an exception when testing
// our condition. NSubstitute will assume it does not match and swallow the exception.
Assert.AreNotEqual("matched", formatter.Format(null));
```

## Matching a specific argument
An argument of type `T` can be matched using `Arg.Is<T>(T value)`.

```csharp
calculator.Add(0, 42);

//This won't work; NSubstitute isn't sure which arg the matcher applies to:
//calculator.Received().Add(0, Arg.Any<int>());

calculator.Received().Add(Arg.Is(0), Arg.Any<int>());
```

This matcher normally isn't required; most of the time we can just use `0` instead of `Arg.Is(0)`. In some cases though, NSubstitute can't work out which matcher applies to which argument (arg matchers are actually fuzzily matched; not passed directly to the function call). In these cases it will throw an `AmbiguousArgumentsException` and ask you to specify one or more additional argument matchers. In some cases you may have to explicitly use argument matchers for every argument.

## Matching `out` and `ref` args

Argument matchers can also be used with `out` and `ref` (NSubstitute 4.0 and later with C# 7.0 and later).

```csharp
calculator
    .LoadMemory(1, out Arg.Any<int>())
    .Returns(x => {
        x[1] = 42;
        return true;
    });

var hasEntry = calculator.LoadMemory(1, out var memoryValue);
Assert.AreEqual(true, hasEntry);
Assert.AreEqual(42, memoryValue);
```

See [Setting out and ref args](/help/setting-out-and-ref-arguments/) for more information on working with `out` and `ref`.

## How NOT to use argument matchers

Occasionally argument matchers get used in ways that cause unexpected results for people. Here are the most common ones.

### Using matchers outside of stubbing or checking received calls

Argument matchers should only be used when specifying calls for the purposes of setting return values, checking received calls, or configuring callbacks (for example: with `Returns`, `Received` or `When`). Using `Arg.Is` or `Arg.Any` in other situations can cause your tests to behave in unexpected ways.

Argument matchers should only be used for:

* Specifying a call when using `Returns` and `ReturnsForAnyArgs`
* Specifying a call within a `When` or `WhenForAnyArgs` block to configure a callback/call action
* Specifying a call to check with `Received`, `DidNotReceive` and `Received.InOrder`
* Configuring a callback with `Arg.Do` or `Arg.Invoke`

Using an argument matcher without one of these calls is most likely an error.

For example:

```csharp
/* ARRANGE */

var widgetFactory = Substitute.For<IWidgetFactory>();
var subject = new Sprocket(widgetFactory);

// OK: Use arg matcher for a return value:
widgetFactory.Make(Arg.Is<WidgetInfo>(x => x.Quantity > 10)).Returns(TestWidget);

/* ACT */

// NOT OK: arg matcher used with a real call:
//   subject.StartWithWidget(Arg.Any<WidgetInfo>());

// Use a real argument instead:
subject.StartWithWidget(new WidgetInfo { Name = "Test", Quantity = 4 });

/* ASSERT */

// OK: Use arg matcher to check a call was received:
widgetFactory.Received().Make(Arg.Is<WidgetInfo>(x => x.Name == "Test"));
```

In this example it would be an error to use an argument matcher in the `ACT` part of this test. Even if we don't mind what specific argument we pass to our subject, `Arg.Any` is only for substitutes, and only for specifying a call while setting return values, checking received calls or for configuring callbacks; not for real calls.

(If you do want to indicate to readers that the precise argument used for a real call doesn't matter you could use a variable such as `var someWidget = new WidgetInfo(); subject.StartWithWidget(someWidget);` or similar. Just stay clear of argument matchers for this!)

Similarly, we should not use an arg matcher as a real value to return from a call (even a substituted one):

```csharp
var widgetFactory = Substitute.For<IWidgetFactory>();

// NOT OK: using an arg matcher as a value, not to specify a call:
//    widgetFactory.MakeDefaultWidget().Returns(Arg.Any<string>());

// Instead use something like:
widgetFactory.MakeDefaultWidget().Returns("any widget");
```

Another legal use of argument matchers is specifying calls when configuring callbacks:

```csharp
/* ARRANGE */
var widgetFactory = Substitute.For<IWidgetFactory>();
var subject = new Sprocket(widgetFactory);

// OK: Use arg matcher to configure a callback:
var testLog = new List<string>();
widgetFactory.When(x => x.Make(Arg.Any<WidgetInfo>())).Do(x => testLog.Add(x.Arg<WidgetInfo>().Name));

// OK: Use Arg.Do to configure a callback:
var testLog2 = new List<string>();
widgetFactory.Make(Arg.Do<WidgetInfo>(info => testLog2.Add(info.Name)));

/* ACT */
subject.StartWithWidget(new WidgetInfo { Name = "Test Widget" });

/* ASSERT */
Assert.AreEqual(new[] { "Test Widget" }, testLog);
Assert.AreEqual(new[] { "Test Widget" }, testLog2);
```

### Modifying values being matched

When NSubstitute records calls, it keeps a reference to the arguments passed, not a deep clone of each argument at the time of the call. This means that if the properties of an argument change after the call assertions may not behave as expected.

<!--
```requiredcode
public interface IPersonLookup {
    void Add(Person p);
}
public interface IPersonStructLookup {
    void Add(PersonStruct p);
}
```
-->

```csharp
public class Person {
    public string Name { get; set; }
}

[Test]
public void MutatingAMatchedArgument() {
    var person = new Person { Name = "Carrot" };
    var lookup = Substitute.For<IPersonLookup>();

    // Called with a Person that has a .Name property of "Carrot"
    lookup.Add(person);

    // The Name in that person reference later gets updated ...
    person.Name = "Vimes";

    // When the substitute is queried, it will check the fields of the person reference it was called with.
    // This means the argument it was called with does NOT have a .Name of "Carrot" (it was changed!)
    lookup.DidNotReceive().Add(Arg.Is<Person>(p => p.Name == "Carrot"));
    // Instead, it now has the updated name:
    lookup.Received().Add(Arg.Is<Person>(p => p.Name == "Vimes"));
}
```

This looks confusing at first, but if we remember substitutes are pretty much forced to store references to arguments used then it makes sense. The alternative of storing deep-cloned snapshots of every argument to every call received is fairly impractical, especially if we consider objects with very complex hierarchies (e.g. tens of fields, each with an object with tens of fields of its own, etc.). Storing snapshots would also lead to the same confusion in the reverse situation, where we know a substitute was called with a particular reference but the `Arg.Is(person)` check fails due to a change in one of its fields.

That said, there are times when snapshots like this are useful, and there are a few ways to enable this with NSubstitute.

The first option is to use structs instead of classes for these cases. These are passed by value rather than by reference, so that value will be stored by substitutes and modifications made afterwards will not affect that value.

```csharp
public struct PersonStruct {
    public string Name { get; set; }
}

[Test]
public void MutatingAStruct() {
    var person = new PersonStruct { Name = "Carrot" };
    var lookup = Substitute.For<IPersonStructLookup>();

    lookup.Add(person);

    person.Name = "Vimes";

    // `person` was passed by value, and that value still has the original Name
    lookup.Received().Add(Arg.Is<PersonStruct>(p => p.Name == "Carrot"));
}
```


For cases where that is not possible or wanted then we can manually snapshot the values we are interested in.

```csharp
[Test]
public void ManualArgSnapshot() {
    var person = new Person { Name = "Carrot" };
    var lookup = Substitute.For<IPersonLookup>();
    var namesAdded = new List<string>();
    // Manually snapshot the value or values we care about:
    lookup.Add(Arg.Do<Person>(p => namesAdded.Add(p.Name)));


    lookup.Add(person);
    person.Name = "Vimes";

    Assert.AreEqual("Carrot", namesAdded[0]);
}
```

 We can then use our standard assertion library for checking the value. This approach can also be helpful for asserting on complex objects, as our assertions can be more detailed and provide more useful information than NSubstitute typically provides in these cases.