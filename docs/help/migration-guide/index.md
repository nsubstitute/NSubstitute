---
title: Migration guide
---

This guide helps you migrate between major versions of NSubstitute and understand breaking changes.

## Migrating to NSubstitute 5.x from 4.x

### .NET Framework support changes

**NSubstitute 5.0** dropped support for older .NET Framework versions:

- ❌ Removed: .NET Framework 4.6.1 and earlier
- ✅ Minimum: .NET Framework 4.6.2
- ✅ Added: .NET 6.0 support
- ✅ Continued: .NET Standard 2.0 support

**Action required**: Update your project to target .NET Framework 4.6.2 or later, or migrate to .NET Core/.NET 5+.

### Obsolete API removals

Several APIs marked as obsolete in 4.x were removed in 5.0:

#### Removed: `Raise.Event()` without generic parameter

```csharp
// ❌ No longer works (removed):
calculator.PoweringUp += Raise.Event();

// ✅ Use instead:
calculator.PoweringUp += Raise.Event<Action>();
```

#### Removed: `SubstituteExtensions.Returns()` overloads

Some less-commonly used `Returns()` overloads were consolidated:

```csharp
// ❌ Removed overload:
substitute.Method().Returns(x => result, x => result2);

// ✅ Use instead:
substitute.Method().Returns(result, result2);
// or
substitute.Method().Returns(x => result);
substitute.Method().Returns(x => result2);
```

### Performance improvements

NSubstitute 5.0 includes significant performance improvements:

- **Faster substitute creation** for interfaces
- **Reduced memory allocation** in call recording
- **Improved argument matching** performance

No code changes required - these improvements are automatic.

## Migrating to NSubstitute 4.x from 3.x

### Breaking changes in 4.0

#### Argument matcher changes

**Stricter type checking** for argument matchers:

```csharp
// ❌ This may have worked in 3.x but fails in 4.x:
substitute.Method(Arg.Any<object>()).Returns("result");
substitute.Method("string argument"); // Won't match in 4.x

// ✅ Be more specific with types:
substitute.Method(Arg.Any<string>()).Returns("result");
```

#### CallInfo argument access

**Index-based access** became more strict:

```csharp
// ❌ May fail in 4.x if types don't match exactly:
substitute.Method(Arg.Any<string>(), Arg.Any<int>()).Returns(x =>
{
    var str = (string)x[0]; // Risky cast
    var num = (int)x[1];    // Risky cast
    return $"{str}-{num}";
});

// ✅ Use type-safe access:
substitute.Method(Arg.Any<string>(), Arg.Any<int>()).Returns(x =>
{
    var str = x.Arg<string>();  // Type-safe
    var num = x.ArgAt<int>(1);  // Type-safe with position
    return $"{str}-{num}";
});
```

### New features in 4.x

#### Enhanced async support

Better support for `ValueTask` and `ValueTask<T>`:

```csharp
// ✅ New in 4.x - ValueTask support:
substitute.GetDataAsync().Returns(new ValueTask<string>("data"));

// ✅ Async enumerable support:
substitute.GetItemsAsync().Returns(AsyncEnumerable.Empty<Item>());
```

#### Improved exception handling

```csharp
// ✅ Better exception information in 4.x:
try
{
    substitute.Received().Method(arg);
}
catch (ReceivedCallsException ex)
{
    // More detailed information about mismatched calls
    Console.WriteLine(ex.Message);
}
```

## Migrating to NSubstitute 3.x from 2.x

### Major changes in 3.0

#### .NET Standard support

NSubstitute 3.0 was rewritten to support .NET Standard:

- ✅ Added: .NET Standard 1.3 and 2.0 support
- ✅ Added: .NET Core support
- ❌ Removed: .NET Framework 3.5 support

#### CallInfo enhancements

**New type-safe argument access**:

```csharp
// ✅ New in 3.x (recommended):
substitute.Method(Arg.Any<string>()).Returns(x => x.Arg<string>().ToUpper());

// ⚠️ Still works but not recommended:
substitute.Method(Arg.Any<string>()).Returns(x => ((string)x[0]).ToUpper());
```

#### Improved argument matching

```csharp
// ✅ Enhanced argument matching in 3.x:
substitute.Method(Arg.Is<User>(u => u.IsActive && u.Age > 18)).Returns(true);
```

## General migration strategies

### Step-by-step migration process

1. **Update NSubstitute version** in your package references
2. **Build your project** and note any compilation errors
3. **Run your tests** to identify runtime issues
4. **Update obsolete API usage** following compiler warnings
5. **Test thoroughly** to ensure behavior hasn't changed

### Common migration patterns

#### Replace obsolete APIs

```csharp
// Pattern 1: Event raising
// Old: Raise.Event()
// New: Raise.Event<TEventHandler>()

// Pattern 2: Argument access
// Old: x[index] with casting
// New: x.Arg<T>() or x.ArgAt<T>(index)

// Pattern 3: Return value configuration
// Old: Complex overloads
// New: Simplified, consistent API
```

#### Update project files

**Package.config** (legacy):
```xml
<package id="NSubstitute" version="5.0.0" targetFramework="net472" />
```

**PackageReference** (modern):
```xml
<PackageReference Include="NSubstitute" Version="5.0.0" />
```

### Compatibility helpers

Create extension methods to ease migration:

```csharp
public static class NSubstituteMigrationHelpers
{
    // Helper for migrating from old event syntax
    public static void RaiseEventLegacy<T>(this T substitute, string eventName) 
        where T : class
    {
        // Implementation for backward compatibility
        substitute.GetType()
            .GetEvent(eventName)
            ?.GetAddMethod()
            ?.Invoke(substitute, new object[] { Raise.Event<Action>() });
    }
}
```

## Version-specific considerations

### NSubstitute 5.x

**Best for**: New projects, .NET 6+ applications
**Consider if**: You need the latest performance improvements and features

### NSubstitute 4.x

**Best for**: Projects that need modern features but must support older .NET Framework versions
**Consider if**: You're upgrading from 3.x and need async enhancements

### NSubstitute 3.x

**Best for**: Stable projects that work well with current features
**Consider if**: You're not ready for the breaking changes in 4.x+

## Testing your migration

### Migration test checklist

- [ ] All projects build without warnings
- [ ] All existing tests pass
- [ ] No performance regressions in test execution
- [ ] Memory usage hasn't increased significantly
- [ ] New version-specific features work as expected

### Common migration issues

**Issue**: Tests fail with argument matching errors
**Solution**: Review argument matchers for type specificity

**Issue**: Performance degradation
**Solution**: Check for unnecessary recreating of substitutes

**Issue**: Memory leaks in long-running test suites
**Solution**: Ensure proper cleanup of substitute references

## Getting help with migrations

If you encounter issues during migration:

1. **Check the [changelog](https://github.com/nsubstitute/NSubstitute/blob/main/CHANGELOG.md)** for detailed breaking changes
2. **Search [GitHub issues](https://github.com/nsubstitute/NSubstitute/issues)** for similar migration problems
3. **Create a minimal reproduction** of the issue
4. **Ask for help** on [Stack Overflow](https://stackoverflow.com/questions/tagged/nsubstitute) with the `nsubstitute` tag

## Related topics

* [Getting started](/help/getting-started/) - for new projects
* [Troubleshooting](/help/troubleshooting/) - for resolving issues
* [Advanced usage](/help/advanced-usage/) - for complex scenarios