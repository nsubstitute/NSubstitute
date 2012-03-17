using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
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
#if SILVERLIGHT
                return Arg<ICall>.Matches<ICall>(x => CalledWithOnlyOneArg(x, arg) && CallIsInvokeOnDelegateCall<string>(x)); 
#else
                return Arg<ICall>.Matches(x => CalledWithOnlyOneArg(x, arg) && CallIsInvokeOnDelegateCall<string>(x)); 
#endif
            }

            private bool CallIsInvokeOnDelegateCall<T>(ICall x)
            {
                return x.GetMethodInfo() == typeof(DelegateCall).GetMethod("GenericInvoke").MakeGenericMethod(typeof(T));
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

        public class When_generating_a_proxy_for_a_delegate_and_specifying_other_interfaces : Concern
        {
            [Test]
            public void Should_throw_substitute_exception()
            {
                Assert.Throws<SubstituteException>(() => sut.GenerateProxy(_callRouter, typeof(Func<int>), new[] {typeof(IFoo)}, null));
            }
        }

        public class When_generating_a_proxy_for_a_delegate_and_specifying_constructor_arguments : Concern
        {
            [Test]
            public void Should_throw_substitute_exception()
            {
                Assert.Throws<SubstituteException>(() => sut.GenerateProxy(_callRouter, typeof(Func<int>), null, new[] { new object() }));
            }
        }

    }
}