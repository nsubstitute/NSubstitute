using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class SubstituteFactorySpecs
    {
        public class Concern : ConcernFor<SubstituteFactory>
        {
            protected ICallRouterFactory _callRouterFactory;
            protected ISubstitutionContext _context;
            protected IProxyFactory _proxyFactory;
            protected ICallRouterResolver _callRouterResolver;

            public override void Context()
            {
                base.Context();
                _context = mock<ISubstitutionContext>();
                _callRouterFactory = mock<ICallRouterFactory>();
                _proxyFactory = mock<IProxyFactory>();
                _callRouterResolver = mock<ICallRouterResolver>();
            }

            public override SubstituteFactory CreateSubjectUnderTest()
            {
                return new SubstituteFactory(_context, _callRouterFactory, _proxyFactory, _callRouterResolver);
            }
        }

        public class When_creating_a_substitute_for_types_that_include_a_class : Concern
        {
            Foo _proxy;
            object _result;
            ICallRouter _callRouter;
            private Type[] _types;
            private object[] _constructorArgs;

            [Test]
            public void Should_return_a_proxy_for_the_class()
            {
                Assert.That(_result, Is.SameAs(_proxy));
            }

            [Test]
            public void Should_register_proxy_and_call_router_with_call_router_resolver()
            {
                _callRouterResolver.received(x => x.Register(_proxy, _callRouter));
            }

            public override void Because()
            {
                _result = sut.Create(_types, _constructorArgs);
            }

            public override void Context()
            {
                base.Context();
                _proxy = new Foo();
                _callRouter = mock<ICallRouter>();
                _types = new[] { typeof(IFoo), typeof(Foo) };
                _constructorArgs = new[] { new object() };

                _callRouterFactory.stub(x => x.Create(_context)).Return(_callRouter);
                _proxyFactory.stub(x => x.GenerateProxy(_callRouter, typeof(Foo), new[] { typeof(IFoo) }, _constructorArgs)).Return(_proxy);
            }
        }

        public class When_getting_the_call_router_created_for_a_substitute : Concern
        {
            private ICallRouter _result;
            private object _substitute;
            private object _callRouter;

            [Test]
            public void Should_resolve_call_router()
            {
                Assert.That(_result, Is.SameAs(_callRouter));
            }

            public override void Because()
            {
                _result = sut.GetCallRouterCreatedFor(_substitute);
            }

            public override void Context()
            {
                base.Context();
                _callRouter = mock<ICallRouter>();
                _substitute = new object();
                _callRouterResolver.stub(x => x.ResolveFor(_substitute)).Return(_callRouter);
            }
        }
    }
}