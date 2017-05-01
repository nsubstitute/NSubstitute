---
title: Setting out and ref args
layout: post
---

`Out` and `ref` arguments can be set using a [`Returns()` callback](/help/return-from-function), or using [`When..Do`](/help/callbacks).

{% examplecode csharp %}
public interface ILookup {
    bool TryLookup(string key, out string value);
}
{% endexamplecode %}

For the interface above we can configure the return value and set the output of the second argument like this:

{% examplecode csharp %}
//Arrange
var value = "";
var lookup = Substitute.For<ILookup>();
lookup
    .TryLookup("hello", out value)
    .Returns(x => { 
        x[1] = "world!";
        return true;
    });

//Act
var result = lookup.TryLookup("hello", out value);

//Assert
Assert.True(result);
Assert.AreEqual(value, "world!");
{% endexamplecode %}



