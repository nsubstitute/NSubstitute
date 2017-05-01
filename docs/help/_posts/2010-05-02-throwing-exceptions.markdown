---
title: Throwing exceptions
layout: post
---

[Callbacks](/help/callbacks) can be used to throw exceptions when a member is called.

{% requiredcode %}
public interface ICalculator { int Add(int a, int b); }
ICalculator calculator;
[SetUp] public void SetUp() { calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %}

{% examplecode csharp %}
//For non-voids:
calculator.Add(-1, -1).Returns(x => { throw new Exception(); });

//For voids and non-voids:
calculator
    .When(x => x.Add(-2, -2))
    .Do(x => { throw new Exception(); });

//Both calls will now throw.
Assert.Throws<Exception>(() => calculator.Add(-1, -1));
Assert.Throws<Exception>(() => calculator.Add(-2, -2));
{% endexamplecode %}

