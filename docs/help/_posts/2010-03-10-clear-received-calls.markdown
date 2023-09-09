---
title: Clearing received calls
layout: post
---

A substitute can forget all the calls previously made to it using the `ClearReceivedCalls()` extension method.

Say we have an `ICommand` interface, and we want a `OnceOffCommandRunner` that will take an `ICommand` and only run it once.

```csharp
public interface ICommand {
    void Execute();
}

public class OnceOffCommandRunner {
    ICommand command;
    public OnceOffCommandRunner(ICommand command) {
        this.command = command;
    }
    public void Run() {
        if (command == null) return;
        command.Execute();
        command = null;
    }
}
```

If we substitute for `ICommand` we can test it is called on the first run, then forget any previous calls made to it, and make sure it is not called again.

```csharp
var command = Substitute.For<ICommand>();
var runner = new OnceOffCommandRunner(command);

//First run
runner.Run();
command.Received().Execute();

//Forget previous calls to command
command.ClearReceivedCalls();

//Second run
runner.Run();
command.DidNotReceive().Execute();
```

`ClearReceivedCalls()` will not clear any results set up for the substitute using `Returns()`. If we need to this, we can [replace previously specified results](/help/replacing-return-values) by calling `Returns()` again.