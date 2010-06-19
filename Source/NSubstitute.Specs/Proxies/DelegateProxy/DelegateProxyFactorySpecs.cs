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
        public abstract class Concern : ConcernFor<DelegateProxyFactory>
        {
            protected ICallRouter _callRouter;

            public override void Context()
            {
                _callRouter = mock<ICallRouter>();
            }

            public override DelegateProxyFactory CreateSubjectUnderTest()
            {
                return new DelegateProxyFactory();
            }
        }

        public class When_generating_a_proxy_for_a_delegate : Concern
        {
            private Func<int, string> _result;

            [Test]
            public void Proxy_should_forward_calls_to_call_router()
            {
                _result(12);
                _callRouter.received(x => x.Route(DelegateCallWithArg(12)));
            }

            public override void Because()
            {
                _result = (Func<int, string>) sut.GenerateProxy(_callRouter, typeof(Func<int, string>), null, null);
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

        public class When_generating_a_proxy_for_an_action : Concern
        {
            [Test]
            public void Should_be_able_to_create_an_action_proxy()
            {
                var result = (Action<int>) sut.GenerateProxy(_callRouter, typeof(Action<int>), null, null);
                result(12);
            }           
        }

    }
}