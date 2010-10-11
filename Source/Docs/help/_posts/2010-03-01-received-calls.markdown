---
title: Checking received calls
layout: post
---

In some cases (particularly for `void` methods) it is useful to check that a specific call has been received by a substitute. This can be checked using the `Received()` extension method, followed by the call being checked.

{% examplecode csharp %}
public interface ICommand {
    void Execute();
}
{% endexamplecode %}

{% examplecode csharp %}
//Arrange
var command = Substitute.For<ICommand>();
//Act
command.Execute();
//Assert
command.Received().Execute();
{% endexamplecode %}

In this case `command` did receive a call to `Execute()`, and so will complete successfully. If `Execute()` has not been received NSubstitute will throw a `CallNotReceivedException` and let you know what call was expected and with which arguments, as well as listing actual calls to that method and which the arguments differed.
