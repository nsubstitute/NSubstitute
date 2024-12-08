---
title: Partial subs and test spies
---

Partial substitutes allow us to create an object that acts like a real instance of a class, and selectively substitute for specific parts of that object. This is useful for when we need a substitute to have real behaviour except for a single method that we want to replace, or when we just want to spy on what calls are being made.

**WARNING:** Partial substitutes will be calling your class' **real code by default**, so if you are not careful it is possible for this code to run **even while you are configuring specific methods to be substituted**! For this reason partial substitutes are not generally recommended, so avoid them where possible (especially if your code deletes files, contacts payment gateways, or initiates underground lair self-destruct routines). In some cases they can be quite handy though; just be sure to handle with care.

## Replacing a single method

In this example we want to test the `Read()` method logic without running `ReadFile()`.

```csharp
public class SummingReader {
  public virtual int Read(string path) {
    var s = ReadFile(path);
    return s.Split(',').Select(int.Parse).Sum();
  }
  public virtual string ReadFile(string path) { return "the result of reading the file here"; }
}
```

By default `ReadFile` may access a file on the file system, but we can `Substitute.ForPartsOf<SummingReader>()` and override `ReadFile` to return a substitute value, rather than loading data from a real file, using `Returns`:

```csharp
[Test]
public void ShouldSumAllNumbersInFile() {
  var reader = Substitute.ForPartsOf<SummingReader>();
  reader.Configure().ReadFile("foo.txt").Returns("1,2,3,4,5"); // CAUTION: real code warning!

  var result = reader.Read("foo.txt");

  Assert.That(result, Is.EqualTo(15));
}
```

Now the real `Read` method will execute, but `ReadFile` will return our substituted value instead of calling the original method, so we can run the test without having to worry about a real file system.

Note the **CAUTION** comment. If we had not used [`Configure()`](/help/configure/) here before `.ReadFile()` then the real `ReadFile` method would have executed before we had a chance to override the behaviour (`reader.ReadFile("foo.txt")` returns first before `.Returns(...)` executes). In some cases this may not be a problem, but if in doubt make sure you call `Configure()` first so NSubstitute knows you are configuring a call and don't want to run any real code. (This still does not guarantee real code will not run -- remember, NSubstitute will not prevent non-virtual calls from executing.)

*The `Configure()` method is only available in NSubstitute 4.0 and above. For verisons prior to 4.0 we need to use `When .. DoNotCallBase` described below.*

## Void methods and `DoNotCallBase`

We can't use `.Returns()` with void methods, but we can stop a void method on a partial substitute from calling the real method using `When .. DoNotCallBase`. (This also works for non-void methods, although generally we use `Configure()` and `Returns()` to override the base behaviour in these cases.)

```csharp
public class EmailServer {
  public virtual void Send(string to, string from, string message) {
    // Insert real email sending code here
    throw new NotImplementedException();
  }

  public virtual void SendMultiple(IEnumerable<string> recipients, string from, string message) {
    foreach (var recipient in recipients) {
        Send(recipient, from, message);
    }
  }
}

[Test]
public void ShouldSendMultipleEmails() {
  var server = Substitute.ForPartsOf<EmailServer>();
  server.WhenForAnyArgs(x => x.Send(default, default, default)).DoNotCallBase(); // Make sure Send won't call real implementation

  server.SendMultiple(
    new [] { "alice", "bob", "charlie" },
    "nsubstitute",
    "Partial subs should be used with caution."); // This won't run the real Send now, thanks to DoNotCallBase().

  server.Received().Send("alice", "nsubstitute", Arg.Any<string>());
  server.Received().Send("bob", "nsubstitute", Arg.Any<string>());
  server.Received().Send("charlie", "nsubstitute", Arg.Any<string>());
}
```

## Test spies

Even without substituting for specific parts of a class, the instance returned by `Substitute.ForPartsOf<T>` records all calls made to virtual members, so we can [check `Received()` calls](/help/received-calls/) made to any partial substitute.