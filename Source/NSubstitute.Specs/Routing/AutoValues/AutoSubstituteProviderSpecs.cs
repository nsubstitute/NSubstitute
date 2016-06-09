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
        [TestCase(typeof(TestClasses.PureVirtualClassWithoutParameterlessConstructor))]
        [TestCase(typeof(TestClasses.ClassWithANonVirtualPublicMember))]
        [TestCase(typeof(TestClasses.ClassWithNonVirtualInterfaceImpl))]
        [TestCase(typeof(TestClasses.ImpureDescendentOfPureVirtualClass))]
        [TestCase(typeof(TestClasses.VirtualClassWithInternalConstructor))]
        [TestCase(typeof(TestClasses.SealedClassWithoutMethods))]
        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        [TestCase(typeof(int[]))]
        [TestCase(typeof(int))]
        public void Provides_no_value_for_non_substitutables(Type type)
        {
            var autoValue = new object();
            _substituteFactory.stub(x => x.Create(new[] { type }, new object[0])).Return(autoValue);

            Assert.That(sut.GetValue(type), Is.EqualTo(Maybe.Nothing<object>()));
        }

        [Test]
        [TestCase(typeof(TestClasses.PureVirtualAbstractClassWithDefaultCtor))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithParameterlessConstructor))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithVirtualInterfaceImpl))]
        [TestCase(typeof(TestClasses.PureDescendentOfPureVirtualClass))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithAPublicField))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithAPublicStaticMethod))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithAPublicStaticField))]
        [TestCase(typeof(TestClasses.PureVirtualClassWithAPublicStaticProperty))]
        [TestCase(typeof(IFoo))]
        [TestCase(typeof(Func<int>))]
        public void Provides_value_for_substitutables(Type type)
        {
            var autoValue = new object();
            _substituteFactory.stub(x => x.Create(new[] { type }, new object[0])).Return(autoValue);

            Assert.That(sut.GetValue(type), Is.EqualTo(Maybe.Just(autoValue)));
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

            public class PureVirtualClassWithAPublicStaticMethod
            {
                public static void StaticMethod() {}
            }

            public class PureVirtualClassWithAPublicStaticProperty
            {
                public static string StaticProperty { get; set; }
            }

            public class PureVirtualClassWithAPublicStaticField
            {
                public string StaticField;
            }

            public sealed class SealedClassWithoutMethods
            {
            }
        }
    }
}