---
title: Checking call order
---

Sometimes calls need to be made in a specific order. Depending on the timing of calls like this is known as _temporal coupling_. Ideally we'd change our design to remove this coupling, but for times when we can't NSubstitute lets us resort to asserting the order of calls.

```csharp
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
```

If the calls were received in a different order then `Received.InOrder` will throw an exception and show the expected and actual calls.

We can use standard [argument matchers](/help/argument-matchers/) to match calls, just as we would when [checking for a single received call](/help/received-calls/).

```csharp
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
```

**Note:** `Received.InOrder` does not track calls to property getters; only method calls (and event subscriptions) are checked. This is by design: a getter is often accessed just to reach a nested substitute (e.g. `a.Nested.Call()`), and requiring that access to happen in an exact position in the sequence would make many legitimate tests painful to write. If you need to assert on a property value, use [`Received()`](/help/received-calls/) or [`Returns`](/help/set-return-value/) separately, outside of `Received.InOrder`.

<!--
```requiredcode
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
```
-->