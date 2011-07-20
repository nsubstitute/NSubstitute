using System;
using NSubstitute.Core;
using NSubstitute.Routing.AutoValues;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.AutoValues
{
    public class AutoSubstituteProviderSpecs : ConcernFor<AutoSubstituteProvider>
    {
        private ISubstituteFactory _substituteFactory;

        [Test]
        public void Can_provide_value_for_interface()
        {
            Assert.That(sut.CanProvideValueFor(typeof (IFoo)));
        }

        [Test]
        public void Can_provide_value_for_delegates()
        {
            Assert.That(sut.CanProvideValueFor(typeof (Func<int>)));
        }

        [Test]
        [TestCase(typeof(TestClasses.PureVirtualAbstractClassWithDefaultCtor), true)]
        [TestCase(typeof(TestClasses.PureVirtualClassWithParameterlessConstructor), true)]
        [TestCase(typeof(TestClasses.PureVirtualClassWithVirtualInterfaceImpl), true)]
        [TestCase(typeof(TestClasses.PureDescendentOfPureVirtualClass), true)]
        [TestCase(typeof(TestClasses.PureVirtualClassWithAPublicField), true)]
        [TestCase(typeof(TestClasses.PureVirtualClassWithoutParameterlessConstructor), false)]
        [TestCase(typeof(TestClasses.ClassWithANonVirtualPublicMember), false)]
        [TestCase(typeof(TestClasses.ClassWithNonVirtualInterfaceImpl), false)]
        [TestCase(typeof(TestClasses.ImpureDescendentOfPureVirtualClass), false)]
        [TestCase(typeof(TestClasses.VirtualClassWithInternalConstructor), false)]
        public void Can_provide_value_for_class_type(Type type, bool shouldProvideValue)
        {
            Assert.That(sut.CanProvideValueFor(type), Is.EqualTo(shouldProvideValue));
        }

        [Test]
        public void Should_not_provide_value_for_object_type_as_by_default_object_methods_are_not_proxied()
        {
            Assert.False(sut.CanProvideValueFor(typeof(object)));
        }

        [Test]
        public void Should_not_provide_value_for_string()
        {
            Assert.False(sut.CanProvideValueFor(typeof(string)));
        }

        [Test]
        public void Should_not_provide_value_for_array()
        {
            Assert.False(sut.CanProvideValueFor(typeof(int[])));
        }

        [Test]
        public void Should_not_provide_value_for_value_type()
        {
            Assert.False(sut.CanProvideValueFor(typeof(int)));
        }

        [Test]
        public void Should_create_substitute_for_type()
        {
            var autoValue = new object();
            _substituteFactory.stub(x => x.Create(new[] {typeof (IFoo)}, new object[0])).Return(autoValue);

            Assert.That(sut.GetValue(typeof (IFoo)), Is.SameAs(autoValue));
        }

        public override void Context()
        {
            _substituteFactory = mock<ISubstituteFactory>();
        }

        public override AutoSubstituteProvider CreateSubjectUnderTest()
        {
            return new AutoSubstituteProvider(_substituteFactory);
        }

        public class TestClasses
        {
            public abstract class PureVirtualAbstractClassWithDefaultCtor
            {
                public virtual void Method0() { }
                public abstract int Method1();
                protected void ProtectedNonVirtMethod() { }
            }

            public class PureVirtualClassWithParameterlessConstructor
            {
                public PureVirtualClassWithParameterlessConstructor(int i) { }
                public virtual void Method0() { }
                public virtual int Method1(int a) { return 0; }
                protected PureVirtualClassWithParameterlessConstructor() { }
            }

            public class ClassWithANonVirtualPublicMember
            {
                public virtual void Method() { }
                public void NonVirtMethod() { }
            }

            public class PureVirtualClassWithoutParameterlessConstructor
            {
                public PureVirtualClassWithoutParameterlessConstructor(int i) { }
                public virtual void Method0() { }
            }

            public class PureVirtualClassWithAPublicField
            {
                public int Number;
                public virtual void Method() { }
            }

            public class ClassWithNonVirtualInterfaceImpl : IFoo { public void Foo() { } }
            public class PureVirtualClassWithVirtualInterfaceImpl : IFoo { public virtual void Foo() { } }
            public interface IFoo { void Foo(); }

            public class PureDescendentOfPureVirtualClass : PureVirtualClassWithParameterlessConstructor
            {
                public virtual void AnotherMethod() { }
            }

            public class ImpureDescendentOfPureVirtualClass : PureVirtualClassWithParameterlessConstructor
            {
                public void AnotherMethod() { }
            }

            public class VirtualClassWithInternalConstructor { internal VirtualClassWithInternalConstructor() { } }
        }
    }
}