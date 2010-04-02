using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs
{
    public class CallSpecificationFactorySpec
    {
        public abstract class Concern : ConcernFor<CallSpecificationFactory>
        {
            protected ICall _call;
            protected object[] _callArguments;
            protected ISubstitutionContext _context;

            public override void Context()
            {
                base.Context();
                _callArguments = new object[] { 1, "fred" };
                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(mock<MethodInfo>());
                _call.stub(x => x.GetArguments()).Return(_callArguments);
                _context = mock<ISubstitutionContext>();
            }

            public override CallSpecificationFactory CreateSubjectUnderTest()
            {
                return new CallSpecificationFactory(_context);
            }
        }

        public abstract class When_creating_a_specification : Concern
        {
            protected ICallSpecification _result;

            [Test]
            public void Should_set_method_info_on_result()
            {
                Assert.That(_result.MethodInfo, Is.SameAs(_call.GetMethodInfo()));
            }

            public override void Because()
            {
                _result = sut.Create(_call);
            }
        }

        public class When_creating_a_specification_with_no_argument_specifications_in_context : When_creating_a_specification
        {
            public override void Context()
            {
                base.Context();
                _context.Stub(x => x.DequeueAllArgumentSpecifications()).Return(new List<IArgumentSpecification>());
            }

            [Test]
            public void Should_use_basic_equality_specifications_for_all_arguments()
            {
                Assert.That(_result.ArgumentSpecifications.Count, Is.EqualTo(_callArguments.Length));
                Assert.That(_result.ArgumentSpecifications.All(spec => spec is NSubstitute.ArgumentEqualsSpecification));
            }

            [Test]
            public void Should_set_first_argument_matcher_on_result()
            {
                var firstArgumentMatcher = _result.ArgumentSpecifications[0];
                Assert.That(firstArgumentMatcher.IsSatisfiedBy(_callArguments[0]));
                Assert.That(firstArgumentMatcher.IsSatisfiedBy("some other argument"), Is.False);
            }

            [Test]
            public void Should_set_second_argument_matcher_on_result()
            {
                var secondArgumentMatcher = _result.ArgumentSpecifications[1];
                Assert.That(secondArgumentMatcher.IsSatisfiedBy(_callArguments[1]));
                Assert.That(secondArgumentMatcher.IsSatisfiedBy("some other argument"), Is.False);
            }
        }

        public class When_creating_a_specification_with_correct_number_of_argument_specs_from_context : When_creating_a_specification
        {
            private IArgumentSpecification _firstSpec;
            private IArgumentSpecification _secondSpec;

            public override void Context()
            {
                base.Context();
                _firstSpec = mock<IArgumentSpecification>();
                _secondSpec = mock<IArgumentSpecification>();
                _context.Stub(x => x.DequeueAllArgumentSpecifications()).Return(new[] {_firstSpec, _secondSpec}.ToList());
            }

            [Test]
            public void Should_use_argument_specs_from_context()
            {
                Assert.That(_result.ArgumentSpecifications[0], Is.SameAs(_firstSpec));
                Assert.That(_result.ArgumentSpecifications[1], Is.SameAs(_secondSpec));
            }
        }

        public class When_creating_a_specification_and_number_of_argument_specs_from_context_differs_from_arguments_on_call : Concern
        {
            public override void Context()
            {
                base.Context();
                var argSpecFromContext = mock<IArgumentSpecification>();
                _context.Stub(x => x.DequeueAllArgumentSpecifications()).Return(new[] { argSpecFromContext }.ToList());
            }

            [Test]
            public void Should_throw()
            {
                Assert.Throws<AmbiguousParametersException>(
                        () => sut.Create(_call)
                    );
            }
        }
    }
}