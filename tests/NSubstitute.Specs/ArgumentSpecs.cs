using System;
using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ArgumentSpecs
    {
        interface IFoo { }
        class Foo : IFoo { }

        private Argument CreateArgument(Type declaredType, Foo value)
        {
            Func<object> getter = () => value;
            return new Argument(declaredType, getter, null);
        }

        [Test]
        public void Should_return_actual_type_of_argument()
        {
            var arg = CreateArgument(typeof(IFoo), new Foo());
            Assert.That(arg.DeclaredType, Is.EqualTo(typeof(IFoo)));
            Assert.That(arg.ActualType, Is.EqualTo(typeof(Foo)));
        }

        [Test]
        public void For_null_values_should_return_declared_type_as_actual_type()
        {
            var arg = CreateArgument(typeof(IFoo), null);
            Assert.That(arg.DeclaredType, Is.EqualTo(typeof(IFoo)));
            Assert.That(arg.ActualType, Is.EqualTo(typeof(IFoo)));
        }

        [Test]
        public void Can_assign_argument_value_to_type()
        {
            var arg = CreateArgument(typeof(IFoo), new Foo());
            Assert.That(arg.IsValueAssignableTo(typeof(IFoo)));
            Assert.That(arg.IsValueAssignableTo(typeof(Foo)));
            Assert.That(arg.IsValueAssignableTo(typeof(object)));
        }

        [Test]
        public void Can_not_assign_argument_value_to_incompatible_type()
        {
            var arg = CreateArgument(typeof(IFoo), new Foo());
            Assert.False(arg.IsValueAssignableTo(typeof(int)));
        }

        [Test]
        public void Declared_type_is_equal_to_type_that_matches_exactly()
        {
            var arg = CreateArgument(typeof(IFoo), new Foo());
            Assert.That(arg.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(IFoo)), Is.True);
            Assert.That(arg.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(Foo)), Is.False);
        }

        [Test]
        public void Declared_type_that_is_by_ref_is_equal_to_non_by_ref_version_of_that_type()
        {
            var arg = CreateArgument(typeof(IFoo).MakeByRefType(), new Foo());
            Assert.That(arg.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(IFoo)), Is.True);
            Assert.That(arg.IsDeclaredTypeEqualToOrByRefVersionOf(typeof(Foo)), Is.False);
        }

        [Test]
        public void Can_assign_by_ref_argument_to_compatible_type()
        {
            var arg = CreateArgument(typeof(IFoo).MakeByRefType(), new Foo());
            Assert.That(arg.IsValueAssignableTo(typeof(IFoo)));
            Assert.That(arg.IsValueAssignableTo(typeof(Foo)));
            Assert.That(arg.IsValueAssignableTo(typeof(object)));
            Assert.False(arg.IsValueAssignableTo(typeof(int)));
        }
    }
}