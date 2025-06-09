---
title: How NSubstitute works
---

When we substitute for a class or interface, NSubstitute uses the wonderful [Castle DynamicProxy library](https://github.com/castleproject/Core) to generate a new class that inherits from that class or implements that interface. This allows us to use that substitute in place of the original type.

You can think of it working a bit like this:

<!--
```requiredcode
public static void handle_call_invocation() {}
public static int configured_value_for_call() => 42;
```
-->

```csharp
public class Original {
    public virtual int DoStuffWith(string s) => s.Length;
}

// Now if we do:
//      var sub = Substitute.For<Original>();
//
// This is a bit like doing:
public class SubstituteForOriginal : Original {
    public override int DoStuffWith(string s) {
        // Tell NSubstitute to record the call, run when..do actions etc,
        // then return the value configured for this call.
        handle_call_invocation();
        return configured_value_for_call();
    }
}
Original sub = new SubstituteForOriginal();
```

## Calamities with classes

For the case when `Original` is an interface this works perfectly; every member in the interface will be intercepted by NSubstitute's logic for recording calls and returning configured values.

There are some caveats when `Original` is a class though (hence all the [warnings about them in the documentation](/help/creating-a-substitute#substituting-infrequently-and-carefully-for-classes)).

### Non-virtual members

If `DoStuffWith(string s)` is not `virtual`, the `SubstituteForOriginal` class will not be able to override it, so when it is called NSubstitute will not know about it. It is effectively invisible to NSubstitute; it can't record calls to it, it can't configure values using `Returns`, it can't run actions via `When..Do`, it can't verify the call was received. Instead, the real base implementation of the member will run.

This can cause all sorts of problems if we accidentally attempt to configure a non-virtual call, because NSubstitute will get confused about which call you're talking about. Usually this will result in a run-time error, but in the worst case it can affect the outcome of your test, or even the following test in the suite, in non-obvious ways. Thankfully we have [NSubstitute.Analyzers](/help/nsubstitute-analysers) to detect these cases at compile time.

### Internal members and types

Similar limitations apply to `internal` members and types. Because `SubstituteForOriginal` gets generated in a separate assembly, they are invisible to NSubstitute and your test-assembly by default. There are two ways to solve this:

The first possibility is to change the visibility of the member or type that shall be substituted. If you change the visibility of members to [`protected internal`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/protected-internal), they get _visible_ for types that derive from it. The visibility of `internal` types must be set to [`public`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/public). However, keep in mind that changing visibilities should be well considered. The next alternative has less side effects on the maintainability of your project.

The second possibility is to make the `internal` members and types visible to your test-assembly and to the library that NSubstitute uses under the hood. For this you need to _allow_ specific assemblies to accesss the `internal`s. You can do this by either adding an attribute to the assembly that contains the `internal`s or add an annotation to your project file.

#### Option 1. Use an assembly attribute
Add the following code to an arbitrary `.cs`-file of the project that contains the `internal` types.
```
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("name of the assembly that contains your tests")]
```

#### Option 2. Use a tag in the project file (works with .NET 5 and above)
Add an `ItemGroup` that contains the `InternalsVisibleTo`-element beneath the `Project`-element in your `.csproj`-file.
```xml
<Project Sdk="Microsoft.NET.Sdk">
  ...
  <ItemGroup>
    <InternalsVisibleTo Include="DynamicProxyGenAssembly2" />
    <InternalsVisibleTo Include="name of the assembly that contains your tests" />
  </ItemGroup>
</Project>
```

Remember that if members are non-virtual, NSubstitute will not be able to intercept it regardless of whether it is `internal` or `InternalsVisibleTo` has been added.

The good news is that [NSubstitute.Analyzers](/help/nsubstitute-analysers) will also detect attempts to use `internal` members at compile time, and will suggest fixes for these cases.

### Real code

The final thing to notice here is that there is the potential for real logic from the `Original` class to execute. We've already seen how this is possible for non-virtual members, but it can also happen if `Original` has code in its constructor. If the constructor calls `FileSystem.DeleteAllMyStuff()`, then constructing `SubstituteForOriginal` will also run this when the base constructor gets called.

### Class conclusion

* Be careful substituting for classes!
* Where possible use interfaces instead.
* Remember NSubstitute works by inheriting from (or implementing) your original type. If you can't override a member by manually writing a sub-class, then NSubstitute won't be able to either!
* Install [NSubstitute.Analyzers](/help/nsubstitute-analysers) where ever you install NSubstitute. This will help you avoid these (and other) pitfalls.