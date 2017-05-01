---
title: Partial subs and test spies
layout: post
---

Partial substitutes allow us to create an object that acts like a real instance of a class, and selectively substitute for specific parts of that object. This is useful for when we need a substitute to have real behaviour except for a single method that we want to replace, or when we just want to spy on what calls are being made.

**WARNING:** Partial substitutes will be calling your class' **real code by default**, so if you are not careful it is possible for this code to run **even while you are configuring specific methods to be substituted**! For this reason partial substitutes are not generally recommended, so avoid them where possible (especially if your code deletes files, contacts payment gateways, or initiates underground lair self-destruct routines). In some cases they can be quite handy though; just be sure to handle with care.

## Replacing a single method

In this example we want to test the `Read()` method logic without running `ReadFile()`.

{% examplecode csharp %}
public class SummingReader {
  public virtual int Read(string path) {
    var s = ReadFile(path);
    return s.Split(',').Select(int.Parse).Sum();
  }
  public virtual string ReadFile(string path) { return "the result of reading the file here"; }
}
{% endexamplecode %}

By default `ReadFile` may access a file on the file system, but we can `Substitute.ForPartsOf<SummingReader>()` and override `ReadFile` to return a substitute value, rather than loading data from a real file, using `Returns`:

{% examplecode csharp %}
[Test]
public void ShouldSumAllNumbersInFile() {
  var reader = Substitute.ForPartsOf<SummingReader>();
  reader.ReadFile(Arg.Is("foo.txt")).Returns("1,2,3,4,5"); // CAUTION: real code warning!

  var result = reader.Read("foo.txt");

  Assert.That(result, Is.EqualTo(15));
}
{% endexamplecode %}

Now the real `Read` method will execute, but `ReadFile` will return our substituted value instead of calling the original method, so we can run the test without having to worry about a real file system.

Note the **CAUTION** comment. If we had not used an [argument matcher](/help/argument-matchers/) here the real `ReadFile` method would have executed before we had a chance to override the behaviour. This is because `reader.ReadFile("foo.txt")` would run before `.Returns(...)`. In some cases this may not be a problem, but if in doubt make sure you specify an argument matcher (`Arg.Is`, `Arg.Any` etc) so NSubstitute knows you are configuring a call and don't want to run any real code. To play it extra safe, use `When .. DoNotCallBase` as described below.

## Void methods, and the play-it-safe approach to partial subs

We can't use `.Returns()` with void methods, but we can stop a void method from calling the real method using `When .. DoNotCallBase`. This also works with non-void methods and can be handy when we want to have a little more confidence we're not going to run real code (a *little* more confidence -- remember, NSubstitute will not prevent non-virtual calls from executing). The previous example can be rewritten to use this approach:

{% examplecode csharp %}
[Test]
public void ShouldSumAllNumbersInFileATadMoreSafely() {
  var reader = Substitute.ForPartsOf<SummingReader>();
  reader.When(x => x.ReadFile("foo.txt")).DoNotCallBase(); //Make sure the ReadFile call won't call real implementation
  reader.ReadFile("foo.txt").Returns("1,2,3,4,5"); // This won't run the real ReadFile now

  var result = reader.Read("foo.txt");

  Assert.That(result, Is.EqualTo(15));
}
{% endexamplecode %}

## Test spies

Even without substituting for specific parts of a class, the instance returned by `Substitute.ForPartsOf<T>` records all calls made to virtual members, so we can [check `Received()` calls](/help/received-calls/) made to any partial substitute.


