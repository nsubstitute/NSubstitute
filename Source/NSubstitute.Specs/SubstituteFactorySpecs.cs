using System;
using NSubstitute.Core;
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

        public abstract class Concern_for_creating_a_substitute : Concern
        {
            object _proxy;
            object _result;
            ICallRouter _callRouter;
            protected Type[] _types;
            object[] _constructorArgs;

            [Test]
            public void Should_return_a_proxy()
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
                _proxy = new object();
                _constructorArgs = new[] { new object() };
                _callRouter = mock<ICallRouter>();
                _callRouterFactory.stub(x => x.Create(_context)).Return(_callRouter);
            }

            protected void ShouldReturnProxyWhenFactoryCalledWith(Type typeOfProxy, Type[] additionalTypes)
            {
                _proxyFactory.stub(x => x.GenerateProxy(_callRouter, typeOfProxy, additionalTypes, _constructorArgs)).Return(_proxy);
            }
        }

        public class When_creating_a_substitute_for_types_that_include_a_class : Concern_for_creating_a_substitute
        {
            public override void Context()
            {
                base.Context();
                _types = new[] { typeof(IFoo), typeof(Foo) };
                ShouldReturnProxyWhenFactoryCalledWith(typeof(Foo), new[] { typeof(IFoo) });
            }
        }

        public class When_creating_a_substitute_for_types_that_include_a_delegate_type : Concern_for_creating_a_substitute
        {
            public override void Context()
            {
                base.Context();
                _types = new[] { typeof(IFoo), typeof(Foo), typeof(Func<int>) };
                ShouldReturnProxyWhenFactoryCalledWith(typeof(Func<int>), new[] { typeof(IFoo), typeof(Foo) });
            }
        }

        public class When_getting_the_call_router_created_for_a_substitute : Concern
        {
            private ICallRouter _result;
            private object _substitute;
            private ICallRouter _callRouter;

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