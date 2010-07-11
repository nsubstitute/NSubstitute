using System;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routes.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes.Handlers
{
    public class ReturnDefaultForReturnTypeHandlerSpecs
    {
        public class When_handling_a_call_which_returns_a_reference_type : When_handling_a_call_returning<object>
        {
            [Test]
            public void Should_return_null()
            {
                Assert.That(_result, Is.Null);
            }
        }

        public class When_handling_a_call_which_returns_a_value_type : When_handling_a_call_returning<int>
        {
            [Test]
            public void Should_return_default_for_value_type()
            {
                Assert.That(_result, Is.EqualTo(default(int))); 
            }
        }

        public class When_handling_a_call_which_returns_void: When_handling_a_call
        {
            protected override Type ReturnType { get { return typeof (void); } }

            [Test]
            public void Should_return_null()
            {
                Assert.That(_result, Is.Null);
            }
        }

        public abstract class When_handling_a_call : ConcernFor<ReturnDefaultForReturnTypeHandler>
        {
            ICall _call;
            MethodInfo _methodInfo;
            protected object _result;
            protected abstract Type ReturnType { get; }

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _methodInfo = mock<MethodInfo>();
                _methodInfo.stub(x => x.ReturnType).Return(ReturnType);
                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(_methodInfo);
            }

            public override ReturnDefaultForReturnTypeHandler CreateSubjectUnderTest()
            {
                return new ReturnDefaultForReturnTypeHandler();
            }
        }

        public abstract class When_handling_a_call_returning<TReturnType> : When_handling_a_call
        {
            protected override Type ReturnType { get { return typeof (TReturnType); } }
        }

    }
}