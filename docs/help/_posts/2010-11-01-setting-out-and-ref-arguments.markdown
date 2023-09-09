---
title: Setting out and ref args
layout: post
---

`Out` and `ref` arguments can be set using a [`Returns()` callback](/help/return-from-function), or using [`When..Do`](/help/callbacks).

```csharp
public interface ILookup {
    bool TryLookup(string key, out string value);
}
```

For the interface above we can configure the return value and set the output of the second argument like this:

```csharp
//Arrange
var lookup = Substitute.For<ILookup>();
lookup
    .TryLookup("hello", out Arg.Any<string>())
    .Returns(x => { 
        x[1] = "world!";
        return true;
    });

//Act
var result = lookup.TryLookup("hello", out var value);

//Assert
Assert.True(result);
Assert.AreEqual(value, "world!");
```

## Matching after assignments

Be careful when using an argument matcher with a reference we also assign to. The assignment can cause previously matching arguments to stop matching.

```csharp
var counter = 0;
var value = "";
var lookup = Substitute.For<ILookup>();
lookup
    .TryLookup("hello", out Arg.Is(value)) // value is "", matcher will check for ""
    .Returns(x => { 
        x[1] = "assigned"; // Assign to 2nd arg
        counter++;         // Count this matching call
        return true;
    });

// value is "", this will match!
lookup.TryLookup("hello", out value);
// Call matches, counter is now 1:
Assert.AreEqual(1, counter);

// value is now "assigned" but arg matcher is still looking for "", will NOT match anymore!
lookup.TryLookup("hello", out value);
// Call does NOT match anymore, counter is still 1:
Assert.AreEqual(1, counter);
```