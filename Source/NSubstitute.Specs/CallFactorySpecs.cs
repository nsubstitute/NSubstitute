using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFactorySpecs
    {
        public abstract class Concern_for_creating_a_call : ConcernFor<CallFactory>
        {
            protected ISubstitutionContext _context;
            protected MethodInfo _method;
            protected object[] _args;
            protected object _target;
            protected ICall _result;

            public override void Context()
            {
                _context = mock<ISubstitutionContext>();
                _target = new object();
            }

            public override void Because()
            {
                _result = sut.Create(_method, _args, _target);
            }

            [Test]
            public void Should_set_required_call_properties()
            {
                Assert.That(_result.GetMethodInfo(), Is.EqualTo(_method));
                Assert.That(_result.GetArguments(), Is.EqualTo(_args));
                Assert.That(_result.Target(), Is.SameAs(_target));
            }

            public override CallFactory CreateSubjectUnderTest()
            {
                return new CallFactory(_context);
            }
        }

        public class When_creating_a_call_with_no_arguments : Concern_for_creating_a_call
        {
            public override void Context()
            {
                base.Context();
                _method = ReflectionHelper.GetMethod(() => MethodWithNoArgs());
                _args = new object[0];
            }
            [Test]
            public void Should_not_dequeue_argument_specifications_as_none_would_be_specified()
            {
                _context.did_not_receive(x => x.DequeueAllArgumentSpecifications());
            }

            [Test]
            public void Should_have_no_argument_specifications()
            {
                Assert.That(_result.GetArgumentSpecifications(), Is.Empty);
            }
        }

        public class When_creating_call_with_arguments : Concern_for_creating_a_call
        {
            IList<IArgumentSpecification> _argSpecsFromContext;

            public override void Context()
            {
                base.Context();
                _method = ReflectionHelper.GetMethod(() => MethodWithArgs(1, "a"));
                _args = new object[2];
                _argSpecsFromContext = new List<IArgumentSpecification>();
                _context.stub(x => x.DequeueAllArgumentSpecifications()).Return(_argSpecsFromContext);
            }

            [Test]
            public void Should_use_argument_specs_from_context()
            {
                Assert.That(_result.GetArgumentSpecifications(), Is.SameAs(_argSpecsFromContext));
            }
        }

        static void MethodWithNoArgs() { }
        static void MethodWithArgs(int a, string b) { }
    }
}