NSubstitute
========

### What is it?

NSubstitute is a friendly substitute for .NET mocking frameworks.

It's like a stub with property behaviour.
With nice semantics for setting return values.
It only has one mode - loose semantics, which you can query afterwards.
It's meant to be simple, succinct and pleasant to use.



### NOTE: Framework Multi-Targetting

I'm updating the build process to support multiple framework versions:

####Versions Supported

 + .NET 3.5 - build from command line using:
 
    rake config="NET35"
	
*Status*: builds and passes all tests 
	
 + .NET 4.0 - build from command line using:
 
    rake config="NET40"

*Status*: only builds. Need to add toggle for using NUnit 4.0 test runner. Signing issue with System.Core and ILMerge (disabled currently)

 + Silverlight 4.0 - build from command line using:
 
    rake config="SL40"

*Status*: only builds. NUnit test runner has issue with mscorlib version found.
 
####Differences between  Silverlight and .NET versions 

 + SerializationInfo is internal in Silverlight - so custom exceptions had to be modified when building against the CoreCLR. I suspect this was due to FxCop rules around custom exceptions, so perhaps this will be supported in SL5. This means one test from the .NET specifications is not valid in the Silverlight port. All other tests pass.
 + Castle.Core 2.5.2 for Silverlight was added.
 + An old build of Rhino.Mocks 3.5 for Silverlight was added to get the specifications to pass. Not happy with this - as I can't find the source for it, and it appears to be an unsupported build. An API change around .Matches for the Silverlight port required some compiler includes to get working.
 + RobustThreadLocal<T> - which wraps functionality from the Reactive extensions around accessing a value from multiple threads - is not available in the Silverlight port. This will impact multi-threaded tests until a workaround can be coded up. It depends on may parts of Reactive Extensions - ConcurrentStack<T>, some sealed classes, etc - so cannot be used in isolation currently.
 
####Gotchas

 + Multi-threaded tests will not work currently.
 
####TODO

 + <del>Pull latest changes from trunk to my branch and update</del>
 + Get multi-threaded support for tests into framework.
 + Obtain updated Rhino.Mocks build and kill off workarounds.
 + Ping owners and see what their plans are for the outstanding acceptance tests.
 + Get signed builds back for .NET 4.0
 + Get NUnit test runner working for .NET 4.0 and SL 4.0



### Basic use

Let's say we have a basic calculator interface:

<!-- {% examplecode csharp %} -->
    public interface ICalculator
    {
        int Add(int a, int b);
        string Mode { get; set; }
        event Action PoweringUp;
    }
<!-- {% endexamplecode %} -->
<!-- {% requiredcode %}
    ICalculator _calculator;
    [SetUp]
    public void SetUp() { _calculator = Substitute.For<ICalculator>(); }
{% endrequiredcode %} -->

We can ask NSubstitute to create a substitute instance for this type. We could ask for a stub, mock, fake, spy, test double etc., but why bother when we just want to substitute an instance we have some control over?

<!-- {% examplecode csharp %} -->
    _calculator = Substitute.For<ICalculator>();
<!-- {% endexamplecode %} -->

Now we can tell our substitute to return a value for a call:

<!-- {% examplecode csharp %} -->
    _calculator.Add(1, 2).Returns(3);
    Assert.That(_calculator.Add(1, 2), Is.EqualTo(3));
<!-- {% endexamplecode %} -->

We can check that our substitute received a call, and did not receive others:

<!-- {% examplecode csharp %} -->
    _calculator.Add(1, 2);
    _calculator.Received().Add(1, 2);
    _calculator.DidNotReceive().Add(5, 7);
<!-- {% endexamplecode %} -->

If our Received() assertion fails, NSubstitute tries to give us some help as to what the problem might be:

    NSubstitute.Exceptions.CallNotReceivedException : Expected to receive call:
        Add(1, 2)
    Actually received (non-matching arguments indicated with '*' characters):
        Add(1, *5*)
        Add(*4*, *7*)

We can also work with properties using the Returns syntax we use for methods, or just stick with plain old property setters (for read/write properties):

<!-- {% examplecode csharp %} -->
    _calculator.Mode.Returns("DEC");
    Assert.That(_calculator.Mode, Is.EqualTo("DEC"));

    _calculator.Mode = "HEX";
    Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
<!-- {% endexamplecode %} -->

NSubstitute supports argument matching for setting return values and asserting a call was received:

<!-- {% examplecode csharp %} -->
    _calculator.Add(10, -5);
    _calculator.Received().Add(10, Arg.Any<int>());
    _calculator.Received().Add(10, Arg.Is<int>(x => x < 0));
<!-- {% endexamplecode %} -->

We can use argument matching as well as passing a function to Returns() to get some more behaviour out of our substitute (possibly too much, but that's your call):

<!-- {% examplecode csharp %} -->
    _calculator
       .Add(Arg.Any<int>(), Arg.Any<int>())
       .Returns(x => (int)x[0] + (int)x[1]);
    Assert.That(_calculator.Add(5, 10), Is.EqualTo(15));
<!-- {% endexamplecode %} -->

Returns() can also be called with multiple arguments to set up a sequence of return values.

<!-- {% examplecode csharp %} -->
    _calculator.Mode.Returns("HEX", "DEC", "BIN");
    Assert.That(_calculator.Mode, Is.EqualTo("HEX"));
    Assert.That(_calculator.Mode, Is.EqualTo("DEC"));
    Assert.That(_calculator.Mode, Is.EqualTo("BIN"));
<!-- {% endexamplecode %} -->

Finally, we can raise events on our substitutes (unfortunately C# dramatically restricts the extent to which this syntax can be cleaned up):

<!-- {% examplecode csharp %} -->
    bool eventWasRaised = false;
    _calculator.PoweringUp += () => eventWasRaised = true;

    _calculator.PoweringUp += Raise.Event<Action>();
    Assert.That(eventWasRaised);
<!-- {% endexamplecode %} -->

### Building

If you have Visual Studio 2008 or 2010 you should be able to compile NSubstitute and run the unit tests using the NUnit GUI or console test runner (see the ThirdParty directory).
To do full builds you'll also need Ruby and rake to run the rakefile.

### Getting help

If you have questions or feedback on NSubstitute, head on over to the [NSubstitute discussion group](http://groups.google.com/group/nsubstitute).



