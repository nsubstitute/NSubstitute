using System;
using NSubstitute.Core;
using NSubstitute.Extensions;
using NSubstitute.Specs.Infrastructure;
using NSubstitute.Specs.SampleStructures;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ReturnsForAllFuncExtensionSpecs
    {
        public class When_setting_return_value_with_func_for_all : StaticConcern
        {
            protected IFoo _sub;
            protected ISubstitutionContext _substitutionContext;
            protected ICallRouter _callRouter;
            protected IReturn _returnValueSet;

            [Test]
            public void Should_ask_substitute_context_for_call_router()
            {
                _substitutionContext.received(x => x.GetCallRouterFor(_sub));
            }

            [Test]
            public void Should_tell_call_router_to_set_return_for_type()
            {
                _callRouter.received(x => x.SetReturnForType(It.Is(typeof(IFoo)), It.IsAny<ReturnValueFromFunc<IFoo>>()));
            }

            [Test]
            public void Should_tell_the_call_router_to_return_the_correct_value()
            {
                Assert.That(_returnValueSet, Is.TypeOf<ReturnValueFromFunc<IFoo>>());
                Assert.That(_returnValueSet.ReturnFor(null), Is.SameAs(_sub));
            }

            public override void Context()
            {
                _sub = mock<IFoo>();
                _substitutionContext = mock<ISubstitutionContext>();
                _callRouter = mock<ICallRouter>();
                _substitutionContext.stub(x => x.GetCallRouterFor(_sub))
                                    .IgnoreArguments()
                                    .Return(_callRouter);
                _callRouter.stub(x => x.SetReturnForType(It.IsAny<Type>(), It.IsAny<IReturn>()))
                           .IgnoreArguments()
                           .WhenCalled(x => _returnValueSet = (IReturn)x.Arguments[1]);
                temporarilyChange(() => SubstitutionContext.Current).to(_substitutionContext);
            }

            public override void Because()
            {
                _sub.ReturnsForAll<IFoo>(ci => _sub);
            }
        }
    }
}
