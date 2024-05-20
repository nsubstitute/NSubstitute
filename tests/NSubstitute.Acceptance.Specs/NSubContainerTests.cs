using NSubstitute.Core.DependencyInjection;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class NSubContainerTests
{
    [Test]
    public void ShouldActivateRegisteredType()
    {
        var sut = new NSubContainer().Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        var result = sut.Resolve<ITestInterface>();

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void ShouldThrowIfTryToRegisterTypeWithoutPublicCtors()
    {
        var sut = new NSubContainer();

        var ex = Assert.Throws<ArgumentException>(() => sut.Register<ITestInterface, TestImplNoPublicCtors>(NSubLifetime.Transient));
        Assert.That(ex.Message, Contains.Substring("single public constructor"));
    }

    [Test]
    public void ShouldThrowIfTryToRegisterTypeWithMultipleCtors()
    {
        var sut = new NSubContainer();

        var ex = Assert.Throws<ArgumentException>(() => sut.Register<ITestInterface, TestImplMultipleCtors>(NSubLifetime.Transient));
        Assert.That(ex.Message, Contains.Substring("single public constructor"));
    }

    [Test]
    public void ShouldAllowToRegisterTypeWithMultipleCtorsUsingFactoryMethod()
    {
        var sut = new NSubContainer();

        sut.Register(_ => new TestImplMultipleCtors("42"), NSubLifetime.Transient);

        var result = sut.Resolve<TestImplMultipleCtors>();
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Value, Is.EqualTo("42"));
    }

    [Test]
    public void ShouldResolveDependencies()
    {
        var sut = new NSubContainer();

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        var result = sut.Resolve<ClassWithDependency>();
        Assert.That(result.Dep, Is.AssignableTo<TestImplSingleCtor>());
    }

    [Test]
    public void ShouldAllowToResolveDependencyInFactoryMethod()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        sut.Register(r => new ClassWithDependency(r.Resolve<ITestInterface>()), NSubLifetime.Transient);

        var result = sut.Resolve<ClassWithDependency>();
        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void ShouldReturnNewInstanceForEachRequestForTransientLifetime()
    {
        var sut = new NSubContainer();

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        var result1 = sut.Resolve<ITestInterface>();
        var result2 = sut.Resolve<ITestInterface>();
        Assert.That(result2, Is.Not.SameAs(result1));
    }

    [Test]
    public void ShouldReturnNewInstanceForSameRequestForTransientLifetime()
    {
        var sut = new NSubContainer();
        sut.Register<ClassWithMultipleDependencies, ClassWithMultipleDependencies>(NSubLifetime.Transient);
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        var result = sut.Resolve<ClassWithMultipleDependencies>();
        Assert.That(result.TestInterfaceDep, Is.Not.SameAs(result.ClassWithDependencyDep.Dep));
    }

    [Test]
    public void ShouldReturnSameInstanceForNewRequestForSingletonLifetime()
    {
        var sut = new NSubContainer();

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Singleton);

        var result1 = sut.Resolve<ITestInterface>();
        var result2 = sut.Resolve<ITestInterface>();
        Assert.That(result2, Is.SameAs(result1));
    }

    [Test]
    public void ShouldReturnSameInstanceForSameRequestForSingletonLifetime()
    {
        var sut = new NSubContainer();
        sut.Register<ClassWithMultipleDependencies, ClassWithMultipleDependencies>(NSubLifetime.Transient);
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Singleton);

        var result = sut.Resolve<ClassWithMultipleDependencies>();
        Assert.That(result.TestInterfaceDep, Is.SameAs(result.ClassWithDependencyDep.Dep));
    }

    [Test]
    public void ShouldReturnNewInstanceForNewRequestForPerScopeLifetime()
    {
        var sut = new NSubContainer();

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);

        var result1 = sut.Resolve<ITestInterface>();
        var result2 = sut.Resolve<ITestInterface>();
        Assert.That(result2, Is.Not.SameAs(result1));
    }

    [Test]
    public void ShouldReturnSameInstanceForSameRequestForPerScopeLifetime()
    {
        var sut = new NSubContainer();
        sut.Register<ClassWithMultipleDependencies, ClassWithMultipleDependencies>(NSubLifetime.Transient);
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);

        var result = sut.Resolve<ClassWithMultipleDependencies>();
        Assert.That(result.TestInterfaceDep, Is.SameAs(result.ClassWithDependencyDep.Dep));
    }

    [Test]
    public void ShouldReturnSameInstanceWhenResolvingDependencyInFactoryMethodForPerScopeLifetime()
    {
        var sut = new NSubContainer();
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);
        sut.Register(
            r => new ClassWithMultipleDependencies(r.Resolve<ITestInterface>(), r.Resolve<ClassWithDependency>()),
            NSubLifetime.Transient);

        var result = sut.Resolve<ClassWithMultipleDependencies>();
        Assert.That(result.TestInterfaceDep, Is.SameAs(result.ClassWithDependencyDep.Dep));
    }

    [Test]
    public void ShouldUseNewRegistrationOnRepeatedRegister()
    {
        var sut = new NSubContainer();

        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);
        sut.Register<ITestInterface, TestImplSingleCtor2>(NSubLifetime.Transient);

        var result = sut.Resolve<ITestInterface>();
        Assert.That(result, Is.AssignableTo<TestImplSingleCtor2>());
    }

    [Test]
    public void ShouldCreateNewContainerInstanceOnCustomize()
    {
        var sut = new NSubContainer();

        var sutFork = sut.Customize();

        Assert.That(sutFork, Is.Not.SameAs(sut));
    }

    [Test]
    public void ShouldNotModifyOriginalContainerOnCustomize()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        var sutFork = sut.Customize().Register<ITestInterface, TestImplSingleCtor2>(NSubLifetime.Transient);

        var sutResult = sut.Resolve<ITestInterface>();
        var sutForkResult = sutFork.Resolve<ITestInterface>();
        Assert.That(sutResult, Is.AssignableTo<TestImplSingleCtor>());
        Assert.That(sutForkResult, Is.AssignableTo<TestImplSingleCtor2>());
    }

    [Test]
    public void ShouldReturnFromParentContainerIfNoForkCustomizations()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        var sutFork = sut.Customize().Customize().Customize();

        var result = sutFork.Resolve<ITestInterface>();
        Assert.That(result, Is.AssignableTo<TestImplSingleCtor>());
    }

    [Test]
    public void ShouldUseRegistrationFromForkContainerIfRequestComesFromParentContainerRegistration()
    {
        var sut = new NSubContainer();
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);
        var sutFork = sut.Customize().Register<ITestInterface, TestImplSingleCtor2>(NSubLifetime.Transient);

        var sutForkResult = sutFork.Resolve<ClassWithDependency>();

        Assert.That(sutForkResult.Dep, Is.AssignableTo<TestImplSingleCtor2>());
    }

    [Test]
    public void ShouldFailWithMeaningfulExceptionIfUnableToResolveType()
    {
        var sut = new NSubContainer();

        var ex = Assert.Throws<InvalidOperationException>(() => sut.Resolve<ITestInterface>());
        Assert.That(ex.Message, Contains.Substring("not registered"));
        Assert.That(ex.Message, Contains.Substring(typeof(ITestInterface).FullName));
    }

    [Test]
    public void ShouldReturnSameValueWithinSameExplicitScope()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);
        sut.Register<ClassWithDependency, ClassWithDependency>(NSubLifetime.Transient);

        var scope = sut.CreateScope();
        var result1 = scope.Resolve<ClassWithDependency>();
        var result2 = scope.Resolve<ClassWithDependency>();

        Assert.That(result1, Is.Not.SameAs(result2));
        Assert.That(result1.Dep, Is.SameAs(result2.Dep));
    }

    [Test]
    public void ShouldDecorateTheExistingRegistration()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);

        var sutFork = sut
            .Customize()
            .Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var result = sutFork.Resolve<ITestInterface>();

        Assert.That(result, Is.TypeOf<TestImplDecorator>());
        Assert.That(((TestImplDecorator)result).Inner, Is.TypeOf<TestImplSingleCtor>());
    }

    [Test]
    public void ShouldBePossibleToCreateNestedDecorators()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);

        var sutFork = sut
            .Customize()
            .Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var sutForkFork = sutFork
            .Customize()
            .Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var result = sutForkFork.Resolve<ITestInterface>();

        var mostInner = ((result as TestImplDecorator)?.Inner as TestImplDecorator)?.Inner;
        Assert.That(mostInner, Is.TypeOf<TestImplSingleCtor>());
    }

    [Test]
    public void ShouldFailIfTypeToDecorateDoesNotExist()
    {
        var sut = new NSubContainer();

        var ex = Assert.Throws<ArgumentException>(
            () => sut.Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl)));
        Assert.That(ex.Message, Contains.Substring("implementation is not registered"));
    }

    [Test]
    public void ShouldDecorateWhenRegisteredOnSameContainer()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        sut.Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var result = sut.Resolve<ITestInterface>();

        Assert.That(result, Is.TypeOf<TestImplDecorator>());
    }

    [Test]
    public void ShouldPinDecoratedRegistrationAtRegistrationTime()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);
        var sutFork = sut.Customize().Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));

        // Override registration. Very rare case
        sut.Register<ITestInterface, TestImplSingleCtor2>(NSubLifetime.Transient);
        var result = sutFork.Resolve<ITestInterface>();

        Assert.That(result, Is.TypeOf<TestImplDecorator>());
        Assert.That(((TestImplDecorator)result).Inner, Is.TypeOf<TestImplSingleCtor>());
    }

    [Test]
    public void ShouldUseSameLifetimeForDecorator_TransientCase()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Transient);

        sut.Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var scope = sut.CreateScope();
        var result1 = scope.Resolve<ITestInterface>();
        var result2 = scope.Resolve<ITestInterface>();

        Assert.That(result1, Is.Not.SameAs(result2));
        Assert.That(((TestImplDecorator)result1).Inner, Is.Not.SameAs(((TestImplDecorator)result2).Inner));
    }

    [Test]
    public void ShouldUseSameLifetimeForDecorator_PerScopeCase()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.PerScope);

        sut.Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var scope = sut.CreateScope();
        var result1 = scope.Resolve<ITestInterface>();
        var result2 = scope.Resolve<ITestInterface>();

        Assert.That(result1, Is.SameAs(result2));
        Assert.That(((TestImplDecorator)result1).Inner, Is.SameAs(((TestImplDecorator)result2).Inner));
    }

    [Test]
    public void ShouldUseSameLifetimeForDecorator_SingletonCase()
    {
        var sut = new NSubContainer();
        sut.Register<ITestInterface, TestImplSingleCtor>(NSubLifetime.Singleton);

        sut.Decorate<ITestInterface>((impl, r) => new TestImplDecorator(impl));
        var result1 = sut.Resolve<ITestInterface>();
        var result2 = sut.Resolve<ITestInterface>();

        Assert.That(result1, Is.SameAs(result2));
        Assert.That(((TestImplDecorator)result1).Inner, Is.SameAs(((TestImplDecorator)result2).Inner));
    }

    public interface ITestInterface
    {
    }

    public class TestImplSingleCtor : ITestInterface
    {
    }

    public class TestImplSingleCtor2 : ITestInterface
    {
    }

    public class TestImplMultipleCtors : ITestInterface
    {
        public string Value { get; }

        public TestImplMultipleCtors()
        {
        }

        public TestImplMultipleCtors(string value)
        {
            Value = value;
        }
    }

    public class TestImplNoPublicCtors : ITestInterface
    {
        private TestImplNoPublicCtors()
        {
        }
    }

    public class ClassWithDependency(ITestInterface dep)
    {
        public ITestInterface Dep { get; } = dep;
    }

    public class ClassWithMultipleDependencies(ITestInterface testInterfaceDep, ClassWithDependency classWithDependencyDep)
    {
        public ITestInterface TestInterfaceDep { get; } = testInterfaceDep;
        public ClassWithDependency ClassWithDependencyDep { get; } = classWithDependencyDep;
    }

    public class TestImplDecorator(ITestInterface inner) : ITestInterface
    {
        public ITestInterface Inner { get; } = inner;
    }
}