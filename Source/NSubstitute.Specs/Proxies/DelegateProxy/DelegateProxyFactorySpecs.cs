using System;
using NSubstitute.Core;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs.Proxies.DelegateProxy
{
    public class DelegateProxyFactorySpecs 
    {
        public class When_generating_a_proxy_for_a_delegate : ConcernFor<DelegateProxyFactory>
        {
            private Func<int, string> _result;
            private ICallRouter _callRouter;

            [Test]
            public void Proxy_should_forward_calls_to_call_router()
            {
                _result(12);
                _callRouter.received(x => x.Route(DelegateCallWithArg(12)));
            }

            public override void Because()
            {
                _result = sut.GenerateProxy<Func<int, string>>(_callRouter);
            }

            public override void Context()
            {
                _callRouter = mock<ICallRouter>();
            }

            public override DelegateProxyFactory CreateSubjectUnderTest()
            {
                return new DelegateProxyFactory();
            }

            private ICall DelegateCallWithArg(int arg)
            {
                return Arg<ICall>.Matches(
                    x => 
                    CalledWithOnlyOneArg(x, arg) && 
                    CallIsInvokeOnDelegateCall(x)
                    );                
            }

            private bool CallIsInvokeOnDelegateCall(ICall x)
            {
                return x.GetMethodInfo() == typeof(DelegateCall).GetMethod("Invoke");
            }

            private bool CalledWithOnlyOneArg(ICall x, int arg)
            {
                var callArguments = x.GetArguments();
                return callArguments.Length == 1 && (int)callArguments[0] == arg;
            }
        }
    }
}