using System.Collections;
using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class ArgumentEqualsSpecificationSpecs : StaticConcern
    {
        [Test]
        public void Should_match_when_arguments_have_same_reference()
        {
            var list = new List<int>();
            var list2 = (IList)list;
            Assert.That(CreateSubjectUnderTest<List<int>>(list).IsSatisfiedBy(list2));
        }

        [Test]
        public void Should_match_when_value_type_arguments_have_same_value()
        {            
            Assert.That(CreateSubjectUnderTest<int>(1).IsSatisfiedBy(1));
        }

        [Test]
        public void Should_not_match_when_reference_type_arguments_have_different_references()
        {
            Assert.False(CreateSubjectUnderTest<object>(new object()).IsSatisfiedBy(new object()));
        }

        [Test]
        public void Should_match_when_by_ref_types_have_the_same_reference()
        {
            var value = new object();
            var other = value;
            Assert.That(CreateSubjectForByRefType(ref value).IsSatisfiedBy(other)); 
        }

        [Test]
        public void Should_not_match_when_by_ref_types_have_different_references()
        {
            var value = new object();
            var otherReference = new object();
            Assert.False(CreateSubjectForByRefType(ref value).IsSatisfiedBy(otherReference)); 
        }

        [Test]
        public void Should_match_when_both_arguments_are_null()
        {
            string x = null;
            List<int> y = null;
            Assert.That(CreateSubjectUnderTest<string>(x).IsSatisfiedBy(y));
        }

        public ArgumentEqualsSpecification CreateSubjectUnderTest<T>(object value)
        {
            return new ArgumentEqualsSpecification(value, typeof(T));
        }

        public ArgumentEqualsSpecification CreateSubjectForByRefType<T>(ref T value)
        {
            var parameterType = GetType().GetMethod("MethodWithARefArgument").GetParameters()[0].ParameterType;
            return new ArgumentEqualsSpecification(value, parameterType);
        }

        public void MethodWithARefArgument<T>(ref T arg) { }
    }
}