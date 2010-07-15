using NSubstitute.Core;
using NSubstitute.Routing;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallHandlerFactorySpecs
    {
        public class When_creating_a_call_handler : ConcernFor<CallHandlerFactory>
        {
            private SubstituteState _substituteState;
            private object[] _routeArguments;
            private SampleHandler _result;

            [Test]
            public void Should_create_part_by_injecting_dependencies_from_its_state()
            {
                Assert.That(_result.CallStack, Is.EqualTo(_substituteState.CallStack));
                Assert.That(_result.CallResults, Is.EqualTo(_substituteState.CallResults));
                Assert.That(_result.FromRouteArgument, Is.EqualTo(_routeArguments[0]));
            }

            public override void Because()
            {
                _result = (SampleHandler) sut.CreateCallHandler(typeof(SampleHandler), _substituteState, _routeArguments);
            }

            public override void Context()
            {
                _substituteState = new SubstituteState(SubstitutionContext.Current);
                _routeArguments = new object[] {2};
            }

            public override CallHandlerFactory CreateSubjectUnderTest()
            {
                return new CallHandlerFactory();
            }

            class SampleHandler : ICallHandler
            {
                public ICallStack CallStack { get; private set; }
                public ICallResults CallResults { get; private set; }
                public int FromRouteArgument { get; private set; }

                public SampleHandler(ICallStack callStack, ICallResults results, int fromRouteArgument)
                {
                    CallStack = callStack;
                    CallResults = results;
                    FromRouteArgument = fromRouteArgument;
                }

                public object Handle(ICall call) { return null; }
            }
        }
    }
}