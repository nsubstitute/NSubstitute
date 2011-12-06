using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallStackSpecs
    {
        public abstract class Concern : ConcernFor<CallStack>
        {
            public override CallStack CreateSubjectUnderTest()
            {
                return new CallStack();
            }
        }

        public class When_a_call_has_been_pushed : Concern
        {
            ICall _call;

            [Test]
            public void Should_pop_to_get_that_call_back()
            {
                Assert.That(sut.Pop(), Is.SameAs(_call));   
            }

            public override void Because()
            {
                sut.Push(_call);
            }

            public override void Context()
            {
                base.Context();
                _call = mock<ICall>();
            }
        }

        public class When_the_call_stack_is_empty : Concern
        {
            [Test]
            public void Should_get_a_stack_empty_exception_when_trying_to_pop()
            {
                var exception = Assert.Throws<InvalidOperationException>(() => sut.Pop());
                Assert.That(exception.Message, Is.StringContaining("Stack empty"));
            }
        }

        public class When_calls_are_cleared : Concern
        {
            [Test]
            public void Should_not_have_any_calls_to_pop()
            {
                Assert.Throws<InvalidOperationException>(() => sut.Pop());
            }

            public override void Because()
            {
                sut.Push(mock<ICall>());
                sut.Clear();
            }
        }

        public class When_finding_all_calls : Concern
        {
            private IEnumerable<ICall> _result;
            private ICall _firstCall;
            private ICall _secondCall;

            [Test]
            public void Should_return_all_calls_in_the_order_they_were_received()
            {
                Assert.That(_result.ToArray(), Is.EqualTo(new[] { _firstCall, _secondCall })); 
            }

            public override void Because()
            {
                sut.Push(_firstCall);
                sut.Push(_secondCall);
                _result = sut.AllCalls();
            }

            public override void Context()
            {
                base.Context();
                _firstCall = mock<ICall>();
                _secondCall = mock<ICall>();
            }
        }
    }
}