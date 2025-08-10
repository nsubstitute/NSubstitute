---
title: Troubleshooting
---

This guide covers common issues you might encounter when using NSubstitute and how to resolve them.

## Common exceptions and solutions

### `CouldNotSetReturnDueToNoLastCallException`

**Problem**: You're trying to set a return value but NSubstitute can't find the last call.

```csharp
// This will throw an exception:
var result = substitute.SomeMethod(arg);
result.Returns(42); // ERROR: result is the actual return value, not a call
```

**Solution**: Call the method directly on the substitute when setting returns:

```csharp
// Correct:
substitute.SomeMethod(arg).Returns(42);
```

### `CouldNotSetReturnDueToTypeMismatchException`

**Problem**: The return type doesn't match the method's return type.

```csharp
substitute.GetString().Returns(42); // ERROR: returning int for string method
```

**Solution**: Ensure the return value matches the method's return type:

```csharp
substitute.GetString().Returns("hello"); // Correct
```

### `ReceivedCallsException`

**Problem**: The substitute didn't receive the expected call.

```text
Expected to receive a call matching:
    SomeMethod(1, 2)
Actually received no matching calls.
Received 1 non-matching call:
    SomeMethod(1, *3*)
```

**Solution**: Check argument matching and ensure the call was actually made:

```csharp
// If you need exact arguments:
substitute.Received().SomeMethod(1, 2);

// If you want to ignore some arguments:
substitute.Received().SomeMethod(1, Arg.Any<int>());
```

### `ArgumentIsNotOutOrRefException`

**Problem**: Trying to set an `out` or `ref` argument on a parameter that isn't `out` or `ref`.

**Solution**: Ensure the method signature has `out` or `ref` parameters:

```csharp
// Method signature: bool TryGet(string key, out string value)
substitute.TryGet("key", out Arg.Any<string>()).Returns(x =>
{
    x[1] = "found value";
    return true;
});
```

## Performance issues

### Slow substitute creation

**Problem**: Creating substitutes takes a long time.

**Cause**: Dynamic proxy generation can be slow for complex types or on first use.

**Solutions**:
1. **Cache substitutes** in test setup when possible
2. **Use interfaces** instead of classes when possible
3. **Minimize substitute complexity** - avoid substituting types with many members

```csharp
// Good: Cache in setup
private IService _service;

[SetUp]
public void Setup()
{
    _service = Substitute.For<IService>(); // Create once
}

// Good: Use simple interfaces
public interface ISimpleService
{
    string GetData(int id);
}
```

### Memory leaks in long-running tests

**Problem**: Substitutes holding references to objects longer than expected.

**Solution**: Clear substitute state between tests:

```csharp
[TearDown]
public void TearDown()
{
    // Clear any recorded calls
    substitute.ClearReceivedCalls();
    
    // For long-running test suites, consider recreating substitutes
    substitute = Substitute.For<IService>();
}
```

## Argument matching issues

### Unexpected argument matcher behavior

**Problem**: Argument matchers not working as expected.

```csharp
// This might not work as expected:
substitute.Method(Arg.Is<string>(s => s.Contains("test"))).Returns(42);
substitute.Method("testing"); // Might not match
```

**Debugging steps**:
1. **Check exception details** - they often show what arguments were actually passed
2. **Use simpler matchers** first:

```csharp
// Start simple:
substitute.Method(Arg.Any<string>()).Returns(42);

// Then add specificity:
substitute.Method(Arg.Is<string>(s => s != null && s.Contains("test"))).Returns(42);
```

3. **Verify argument types** match exactly:

```csharp
// Be careful with inheritance:
substitute.Method(Arg.Any<object>()).Returns(42); // Matches any object
substitute.Method(Arg.Any<string>()).Returns(42); // Only matches strings
```

### Array and collection matching

**Problem**: Arrays or collections not matching as expected.

**Solution**: Use appropriate matchers or comparison logic:

```csharp
// For exact array matching:
substitute.ProcessItems(Arg.Is<int[]>(arr => arr.SequenceEqual(new[] {1, 2, 3})));

// For collection contents:
substitute.ProcessItems(Arg.Is<IEnumerable<int>>(items => 
    items.Count() == 3 && items.Contains(1)));
```

## Async/await issues

### Async methods not awaited properly

**Problem**: Async substitute methods not behaving correctly in tests.

```csharp
// Potentially problematic:
substitute.GetDataAsync().Returns(Task.FromResult("data"));
var result = substitute.GetDataAsync(); // Don't forget to await!
```

**Solution**: Always await async calls in tests:

```csharp
substitute.GetDataAsync().Returns(Task.FromResult("data"));
var result = await substitute.GetDataAsync(); // Correct
Assert.AreEqual("data", result);
```

### ConfigureAwait and deadlocks

**Problem**: Tests hanging due to context switching issues.

**Solution**: Use `ConfigureAwait(false)` in library code:

```csharp
// In production code:
public async Task<string> GetDataAsync()
{
    var result = await _service.GetDataAsync().ConfigureAwait(false);
    return result;
}
```

## Class substitution issues

### Non-virtual members

**Problem**: Trying to substitute non-virtual members of a class.

```csharp
public class Service
{
    public string GetData() => "real data"; // Not virtual!
}

var substitute = Substitute.For<Service>();
substitute.GetData().Returns("fake data"); // Won't work!
```

**Solutions**:
1. **Make members virtual**:

```csharp
public class Service
{
    public virtual string GetData() => "real data"; // Now it works
}
```

2. **Extract an interface**:

```csharp
public interface IService
{
    string GetData();
}

var substitute = Substitute.For<IService>(); // Much better
```

3. **Use the analyzer** to catch these issues at compile time:

```xml
<PackageReference Include="NSubstitute.Analyzers.CSharp" Version="1.0.14" PrivateAssets="all" />
```

### Constructor issues

**Problem**: Class constructors being called when creating substitutes.

**Solution**: Provide constructor arguments or use interfaces:

```csharp
// If class needs constructor arguments:
var substitute = Substitute.For<Service>("arg1", 42);

// Better: use interfaces to avoid constructor issues
var substitute = Substitute.For<IService>();
```

## Configuration conflicts

### Multiple return values

**Problem**: Conflicting return value configurations.

```csharp
substitute.GetValue(1).Returns(10);
substitute.GetValue(Arg.Any<int>()).Returns(20); // Which one wins?
```

**Understanding the behavior**: More specific configurations take precedence:
1. Exact argument matches beat argument matchers
2. Later configurations can override earlier ones

**Best practice**: Be explicit about your intentions:

```csharp
// Set general case first:
substitute.GetValue(Arg.Any<int>()).Returns(20);

// Then specific cases:
substitute.GetValue(1).Returns(10); // This will override for argument 1
```

## Debugging tips

### Enable verbose exception details

Use the `ReceivedCallsException` details to understand what happened:

```csharp
try
{
    substitute.Received().SomeMethod(expectedArg);
}
catch (ReceivedCallsException ex)
{
    Console.WriteLine(ex.Message); // Shows actual vs expected calls
    throw;
}
```

### Inspect recorded calls

```csharp
var calls = substitute.ReceivedCalls();
foreach (var call in calls)
{
    Console.WriteLine($"Method: {call.GetMethodInfo().Name}");
    Console.WriteLine($"Arguments: {string.Join(", ", call.GetArguments())}");
}
```

### Use test debugging effectively

1. **Set breakpoints** after configuring substitutes
2. **Step through** the production code to see what calls are made
3. **Check call arguments** in the debugger
4. **Verify return values** are what you expect

## Getting help

If you're still stuck:

1. **Check the [documentation](/help/)** for more detailed examples
2. **Search [existing issues](https://github.com/nsubstitute/NSubstitute/issues)** for similar problems
3. **Ask on [Stack Overflow](https://stackoverflow.com/questions/tagged/nsubstitute)** with the `nsubstitute` tag
4. **Create a [minimal reproducible example](https://stackoverflow.com/help/minimal-reproducible-example)** when reporting issues

## Related topics

* [Creating a substitute](/help/creating-a-substitute/) - choosing between interfaces and classes
* [Argument matchers](/help/argument-matchers/) - flexible argument matching
* [NSubstitute.Analyzers](/help/nsubstitute-analysers/) - compile-time issue detection
* [Threading](/help/threading/) - multi-threaded scenarios