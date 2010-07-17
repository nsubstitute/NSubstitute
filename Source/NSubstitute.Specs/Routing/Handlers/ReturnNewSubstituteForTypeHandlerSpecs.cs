using System;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class ReturnNewSubstituteForTypeHandlerSpecs
    {
        public abstract class When_handling_a_call : ConcernFor<ReturnNewSubstituteForTypeHandler>
        {
            protected RouteAction _result;
            protected ICall _call;
            protected ISubstituteFactory _substituteFactory;
            protected IResultSetter _resultSetter;

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _substituteFactory = mock<ISubstituteFactory>();
                _resultSetter = mock<IResultSetter>();
            }

            public override ReturnNewSubstituteForTypeHandler CreateSubjectUnderTest()
            {
                return new ReturnNewSubstituteForTypeHandler(_substituteFactory, _resultSetter);
            }
        }

        public class When_handling_call_that_returns_an_interface : When_handling_a_call
        {
            private IFoo _newSubstitute;

            [Test]
            public void Should_return_new_substitute_for_type()
            {
                Assert.That(_result.ReturnValue, Is.SameAs(_newSubstitute));     
            }

            [Test]
            public void Should_set_result_for_future_calls()
            {
                _resultSetter
                    .received(x => 
                        x.SetResultForCall(It.Is(_call), It.Matches<IReturn>(arg => arg.ReturnFor(null) == _newSubstitute), It.Is(MatchArgs.AsSpecifiedInCall)));
            }

            public override void Context()
            {
                base.Context();
                _call.stub(x => x.GetReturnType()).Return(typeof(IFoo));
                _newSubstitute = mock<IFoo>();
                _substituteFactory.stub(x => x.Create(new[] { typeof(IFoo) }, new object[0])).Return(_newSubstitute);
            }
        }

        public class When_handling_call_that_does_not_return_an_interface : When_handling_a_call
        {
            [Test]
            public void Should_continue_routing()
            {
                Assert.That(_result, Is.SameAs(RouteAction.Continue())); 
            }

            public override void Context()
            {
                base.Context();
                _call.stub(x => x.GetReturnType()).Return(typeof(int));
            }
        }
    }
}