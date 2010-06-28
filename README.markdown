NSubstitute
========

### What is it?

NSubstitute is a friendly substitute for .NET mocking frameworks.

It's like a stub with property behaviour.  
With nice semantics for setting return values.  
It only has one mode - loose semantics, which you can query afterwards.  
It's meant to be simple, succinct and pleasant to use.  

It's also a work in progress (and in the early stages of progress at that).

### Basic use

Let's say we have a basic calculator interface:

	public interface ICalculator
	{
		void SwitchOn();
		int Add(int a, int b);
		int Subtract(int a, int b);
	}

We can ask NSubstitute to create a substitute instance for this type (you could call it a stub, mock, or fake, but why bother when we just want to substitute in an instance we have some control over?).

	[Test]
	public void Use_a_shiny_new_substitute()
	{
		var calculator = Substitute.For<ICalculator>();
		calculator.SwitchOn();
		Assert.That(calculator.Add(1,2), Is.EqualTo(default(int)));
	}

Now we can tell our substitute to return different values for different calls:

	[Test]
	public void Return_different_values_for_different_arguments()
	{
		var calculator = Substitute.For<ICalculator>();
		calculator.Add(1, 2).Returns(3);
		calculator.Add(20, 30).Returns(50);
		Assert.That(calculator.Add(20, 30), Is.EqualTo(50));
		Assert.That(calculator.Add(1, 2), Is.EqualTo(3));
	}

And we can check that our substitute received a call:

	public void Check_a_call_was_received()
	{
		var calculator = Substitute.For<ICalculator>();
		calculator.Add(1, 2);
		calculator.Received().Add(1, 2);            
	}

### Building

If you have Visual Studio 2008 or 2010 you should be able to compile NSubstitute and run the unit tests using the NUnit GUI or console test runner (see the ThirdParty directory).
To do full builds you'll also need Ruby and rake to run the rakefile.

