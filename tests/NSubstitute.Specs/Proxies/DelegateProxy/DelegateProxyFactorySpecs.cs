using System;
using System.Reflection;
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

            protected ICall CallToMethodWithArg(MethodInfo method, int arg)
            {
                return Arg<ICall>.Matches(x => x.GetMethodInfo() == method && CalledWithOnlyOneArg(x, arg));
            }

            private bool CalledWithOnlyOneArg(ICall x, int arg)
            {
                var callArguments = x.GetArguments();
                return callArguments.Length == 1 && (int)callArguments[0] == arg;
            }
        }

        public class When_generating_a_proxy_for_a_delegate : Concern
        {
            [Test]
            public void Proxy_should_forward_calls_with_generic_return_type_to_call_router_so_router_knows_exact_return_type_expected()
            {
                var result = (Func<int, string>)sut.GenerateProxy(_callRouter, typeof(Func<int, string>), null, null);
                result(12);

                _callRouter.received(x =>
                    x.Route(CallToMethodWithArg(DelegateCall.InvokeMethodWithGenericReturnType.MakeGenericMethod(typeof(string)), 12))
                );
            }
        }

        public class When_generating_a_proxy_for_an_action : Concern
        {
            [Test]
            public void Proxy_should_forward_action_calls_to_call_router()
            {
                var result = (Action<int>)sut.GenerateProxy(_callRouter, typeof(Action<int>), null, null);
                result(12);
                _callRouter.received(x => x.Route(CallToMethodWithArg(DelegateCall.InvokeMethodWithObjectOrVoidReturnType, 12)));
            }
        }

        public class When_generating_a_proxy_for_a_delegate_and_specifying_other_interfaces : Concern
        {
            [Test]
            public void Should_throw_substitute_exception()
            {
                Assert.Throws<SubstituteException>(() => sut.GenerateProxy(_callRouter, typeof(Func<int>), new[] { typeof(IFoo) }, null));
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