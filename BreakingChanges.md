4.0.0 Release
================

Argument matchers (`Arg.Is`, `Arg.Any` etc.) now use [`ref` returns which were introduced in C# 7.0](https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/#user-content-ref-returns-and-locals). This lets NSubstitute have better support for working with `out` and `ref` arguments, but also means that test written using previous NSubstitute versions will now fail to compile with pre-C# 7 compilers with the following error:

> CS7085: By-reference return type 'ref T' is not supported.

Reason: Previous NSubstitute versions had quite limited support for `out` and `ref` arguments. Enhanced support for `out` and `ref` in C# 7 means we are likely to see these being used more frequently. As the vast majority of people using NSubstitute seem to be on C# 7+ (based on [NuGet statistics](https://www.nuget.org/stats/packages/NSubstitute?groupby=Version&groupby=ClientVersion)), we've thought it best to make sure NSubstitute's default behaviour works for these cases.

Workaround: If at all possible please update to a recent version of your .NET compiler (C# 7+, VB 2017, VB 15.3+, F# 4.1+). This should provide support for `ref` returns and make the code compile fine with the new `Arg` methods. These shipped with Visual Studio 2017. Visual Studio for Mac 2017 and JetBrains Rider 2017 also support C# 7+.

If it is not possible for you to use a C# 7-compatible compiler, we have added an `Arg.Compat` class which has all the same members as `Arg`, just without the `ref` return type. If you replace `Arg.` references with `Arg.Compat.` in your project then you can continue to use older compilers with NSubstitute 4.x. Alternatively you can use a `NSubstitute.Compatibility.CompatArg` instance in your fixture which may make migration a bit easier. Both these approaches are described in the [Compatibility argument matchers](https://nsubstitute.github.io/help/compat-args) documentation.

---------------

The argument matcher change to support `out` and `ref` mentioned above can also cause a compilation error if argument matchers are used in expression trees (see issue #471):

> CS8153: An expression tree lambda may not contain a call to a method, property, or indexer that returns by reference

Reason: Previous NSubstitute releases had very limited support for matching `out` and `ref` arguments, and this frequently caused confusion for people. We are always reluctant to introducing breaking changes, but in this case we estimated the benefit of improving support for matching `out` and `ref` arguments would outweigh the downside of no longer being able to use matchers in expression trees.

Workaround:
* Move the NSubstitute statement/assertion outside of the expression tree; or
* Use an `Arg.Compat.` matcher as described above and in the [Compatibility argument matchers](https://nsubstitute.github.io/help/compat-args) documentation.

For example:

```
// Given `void specify(Expression<Action> expectation)`, the following will fail to compile (CS8153):
specify(() => sub.Received().SomeCall(Arg.Any<int>()));

// Workaround 1: move out of expression tree
sub.Received().SomeCall(Arg.Any<int>());
// Workaround 2: use Arg.Compat
specify(() => sub.Received().SomeCall(Arg.Compat.Any<int>()));
```

---------------

Unused argument matchers (`Arg.Is`, `Arg.Any` etc.) will now throw an exception. This normally occurs if an argument matcher was used with a non-virtual call, or with an object that is not a substitute. This may cause existing tests to fail if they were misusing argument matchers in a way that did not cause an obvious problem.

Reason: Previously these were ignored, which could cause confusing test failures in subsequent tests. Now these cases should be picked up earlier and make finding the problem easier. See #89 and #279 for examples.

Workaround: Follow the instructions in the exception to fix any instances of this problem. Note that the cause of this exception can be in a previously executed test.

---------------

Calls made with one or more argument matchers (`Arg.Is` or `Arg.Any`) will no longer return previously configured results. NSubstitute will assume the call is being configured and avoid running logic configured via previous `Returns()` calls.

In most cases this should not affect existing tests, but there are some ambiguously nested configurations involving argument
matchers that can start to fail after this change.

See #345 (https://github.com/nsubstitute/NSubstitute/pull/347) for discussion of this and an example of a test whose behaviour has changed.

Reason: This fixes a number of issues relating to overlapping configurations (#291, #225, #146, #177).

Workaround: Separate any nested configurations that start to fail after this change.
If this is difficult for a specific case, create a GitHub issue with the details and we may be able to assist.

---------------

`ArgumentSpecificationQueue` has been removed. Custom argument specifications are now queued using `ArgMatcher.Enqueue`.

Reason: This was done as part of argument matching changes and refactoring of NSubstitute internals (#426, #404, #438, #477).

Workaround: Replace uses of `ArgumentSpecificationQueue.EnqueueSpecFor` with `ArgumentMatcher.Enqueue`. See [this comment on #438](https://github.com/nsubstitute/NSubstitute/issues/438#issuecomment-463091034) for an example.

---------------

Removed `NSubstitute.Core.Extensions.Zip`.

Reason: Zip is in NET40+ and NetStandard. Was formerly provided for NET35 compatibility.

Workaround: Use `System.Linq.Enumerable.Zip`

---------------

Removed obsolete `NSubstitute.Experimental.Received.InOrder`. (#351)

Reason: previously obsoleted by `NSubstitute.Received.InOrder`.

Workaround: Use `NSubstitute.Received.InOrder`.

3.0.1 Release
================

Signed v3.x package to fix libraries that work with a mix of NSubstitute verisons. See #324.

3.0.0 Release 
================

NOTE: unsigned. Fixed in 3.0.1.
NOTE: Support for NET45 and NET46 restored in 3.1.0.

Dropped support for older .NET platforms (pre-.NET 4.6).

Reason: switching to .NET Standard. This provides support for more platforms and makes the
project easier to maintain.

Workaround: Stay with 2.x versions for older .NET platforms, or migrate to a .NET
Standard 1.3 compatible target such as .NET 4.6 or later. See compatibility matrix:

    https://github.com/dotnet/standard/blob/master/docs/versions.md


1.10.0 Release 
================

Substitutes will now automatically return an empty `IQueryable<T>` for
members that return that type. Tests previously relying on a
substitute `IQueryable<T>` will no longer work properly.

Reason:
- Code that uses an `IQueryable<T>` can now run using the auto-subbed 
value without causing null pointer exceptions (see issue #67).

Fix:
- Avoid mocking `IQueryable<T>` where possible -- configure members
to return a real `IQueryable<T>` instead. If a substitute is required, explicitly configure the call to return a substitute:

    ```
    sub.MyQueryable().Returns(Substitute.For<IQueryable<int>>());
    ```


1.9.1 Release 
================

Substitutes set up to throw exception for methods with return type Task<T> 
cause compilation to fail due to the call being ambiguous (CS0121).
"The call is ambiguous between the following methods or properties:
`.Returns<Task<T>>` and `.Returns<T>`"

Reason:
- To make it easier to stub async methods. See issue #189.

Fix:
- Specify generic type argument explicitly. If Method() returns string:

    Old: `sub.Method().Returns(x => { throw new Exception() });`

    New: `sub.Method().Returns<string>(x => { throw new Exception() });`

1.8.0 Release 
================

Incorrect use of argument matchers outside of a member call, particularly within a
`Returns()`, will now throw an exception (instead of causing unexpected behaviour
in other tests: see https://github.com/nsubstitute/NSubstitute/issues/149).

Reason:
- Prevent accidental incorrect use from causing hard-to-find errors in unrelated tests.

Fix:
- Do not use argument matchers in Returns() or outside of where an argument is normally used.

    Correct use:   `sub.Method(Arg.Any<string>()).Returns("hi")`

    Incorrect use: `sub.Method().Returns(Arg.Any<string>())`


1.7.0 Release
================

Auto-substitute for pure virtual classes with at least one public static method, which
means some methods and properties on substitutes that used to return null by default will now return a new substitute of that type.

Reason:
- eep consistency with the behaviour of other pure virtual classes.

Fix:
- Explicitly return null from methods and property getters when required for a test.
e.g. `sub.Method().Returns(x => null)`;

---------------

Moved `Received.InOrder` feature from `NSubstitute.Experimental` to main `NSubstitute` namespace. Obsoleted original `NSubstitute.Experimental.Received`.

This can result in ambiguous reference compiler errors and obsolete member compiler earnings.

Reason:
- Promoted experimental Received feature to core library.

Fix:
- Import `NSubstitute` namespace instead of `NSubstitute.Experimental`.
(If `NSubstitute` is already imported just delete the `using NSubstitute.Experimental;` line from your fixtures.)


1.5.0 Release
================

The base object methods (`Equals`, `GetHashCode` and `ToString`) for substitute objects of classes that extend those methods now return the result of calling the actual implementation of those methods rather than the default value for the return type. This means that places where you relied on `.Equals` returning `false`, `.ToString` returning `null` and `.GetHashCode` returning `0` because the actual
methods weren't called will now call the actual implementation.

Reason:
- Substitute objects of classes that overrode those methods that were used as parameters for setting up return values or checking received calls weren't able to be directly used within the call, e.g. instead of:

`someObject.SomeCall(aSubstitute).Returns(1);`

You previously needed to have:

`someObject.SomeCall(Arg.Is<TypeBeingSubstituted>(a => a == aSubstitute)).Returns(1);`

However, now you can use the former, which is much more terse and consistent with the way other Returns or Received calls work.
- This means that substitute objects will now always work like .NET objects rather than being inconsistent when the class being substituted overrode any of those base object methods.

Fix:
- There is no workaround to change the behaviour of .Equals, .GetHashCode or .ToString. If you have a use case to change the behaviour of these methods please lodge an issue at the NSubstitute Github site.

---------------

In rare cases the new `Returns()` and `ReturnsForAnyArgs()` overloads can cause compilation to fail due to the call being ambiguous (CS0121). 

Reason:
- The new overloads allow a sequence of callbacks to be used for return values. A common example is return several values, then throwing an exception.

Fix:
- Remove the ambiguity by explicitly casting the arguments types or by using lambda syntax. e.g. `sub.Call().Returns(x => null, x => null)`;


1.4.0 Release
================

Auto-substitute from substitutes of `Func` delegates (following the same rules as auto-subbing for methods and properties). So the delegate returned from `Substitute.For<Func<IFoo>>()` will return a substitute of `IFoo`. This means some substitutes for delegates that used to return null will now return a new substitute.

Reason:
- Reduced setup when substituting for `Func` delegates, and consistency with behaviour for properties and methods. 

Fix:
- Explicitly return null from substitute delegates when required for a test.
e.g. `subFunc().Returns(x => null)`;


1.2.0 Release
================

Auto-substitute for pure virtual classes (in addition to interfaces and delegate types), which
means some methods and properties on substitutes that used to return null by default will now
return a new substitute of that type.

Reason:
 - Cut down the code required to configure substitute members that return interface-like types (e.g. ASP.NET web abstractions like ```HttpContextBase```) which are safe to create and proxy.
- Safe classes are those with all their public methods and properties defined as virtual or abstract, and containing a default, parameterless constructor defined as public or protected.

Fix:
- Explicitly return null from methods and property getters when required for a test.
e.g. ```sub.Method().Returns(x => null)```;


0.9.5 Release
================

`Raise.Event<TEventArgs>(...)` methods renamed to `Raise.EventWith<TEventArgs()`

Reason:
- The `Raise.Event<TEventArgs>()` signature would often conflict with the
- `Raise.Event<THandler>()` method which is used to raise all types of events.
- `Raise.Event<THandler>()` will now always work for any event type, while
- `Raise.EventWith<TEventArgs>()` can be used as a shortcut to raise
- EventHandler-style events with a particular argument.

Fix:
- Replace `Raise.Event<TEventArgs>()` calls with equivalent `Raise.EventWith<TEventArgs>()` call.

---------------
`Raise.Action()` methods removed

Reason:
- The `Raise.Event<THandler>()` method can be used to raise all delegate events, including Actions.
- `Raise.Action()` was removed so there is a consistent way of raising all delegate events.

Fix:
- Replace `Raise.Action()` calls with `Raise.Event<Action>()`.
- Replace `Raise.Action<T>(T arg)` calls with `Raise.Event<Action<T>>(arg)`.
- Replace `Raise.Action<T1,T2>(T1 x, T2 y)` calls with `Raise.Event<Action<T1,T2>>(x, y)`.


0.9.0 Release
================

No breaking changes.
