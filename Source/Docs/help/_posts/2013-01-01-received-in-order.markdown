---
title: Checking call order
layout: post
---

**NOTE: This feature is currently in the `NSubstitute.Experimental` namespace while we experiment with its API and behaviour. You are welcome to try it out, but be aware that it may change in later releases. Please leave feedback on the [discussion group](https://groups.google.com/group/nsubstitute).**

Sometimes calls need to be made in a specific order. Depending on the timing of calls like this is known as _temporal coupling_. Ideally we'd change our design to remove this coupling, but for times when we can't NSubstitute lets us resort to asserting the order of calls.

{% examplecode csharp %}
[Test]
public void TestCommandRunWhileConnectionIsOpen() {
  var connection = Substitute.For<IConnection>();
  var command = Substitute.For<ICommand>();
  var subject = new Controller(connection, command);

  subject.DoStuff();

  Received.InOrder(() => {
    connection.Open();
    command.Run(connection);
    connection.Close();
  });
}
{% endexamplecode %}

If the calls were received in a different order then `Received.InOrder` will throw an exception and show the expected and actual calls.

We can use standard [argument matchers](/help/argument-matchers/) to match calls, just as we would when [checking for a single received call](/help/received-calls/).

{% examplecode csharp %}
[Test]
public void SubscribeToEventBeforeOpeningConnection() {
  var connection = Substitute.For<IConnection>();
  connection.SomethingHappened += () => { /* some event handler */ };
  connection.Open();

  Received.InOrder(() => {
    connection.SomethingHappened += Arg.Any<Action>();
    connection.Open();
  });
}
{% endexamplecode %}

{% requiredcode %}

//Wrapper to call Experimental namespace
public class Received {
  public static void InOrder(Action calls) { Experimental.Received.InOrder(calls); }
}

public class Controller {
  IConnection connection;
  ICommand cmd;
  public Controller(IConnection connection, ICommand cmd) {
    this.connection = connection;
    this.cmd = cmd;
  }

  public void DoStuff() {
    connection.Open();
    cmd.Run(connection);
    connection.Close();
  }
}
public interface IConnection {
  void Open();
  void Close();
  event Action SomethingHappened;
}
public interface ICommand {
  void Run(IConnection c);
}
{% endrequiredcode %}

