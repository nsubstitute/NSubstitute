using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class ReturnDefaultForReturnTypeHandlerSpecs
    {
        public class When_handling_a_call : ConcernFor<ReturnDefaultForReturnTypeHandler>
        {
            private readonly object _expectedDefault = new object();
            private IDefaultForType _defaultForType;
            ICall _call;
            RouteAction _result;

            [Test]
            public void Should_return_default_for_type()
            {
                Assert.That(_result.ReturnValue, Is.SameAs(_expectedDefault));

            }
            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                var methodInfo = ReflectionHelper.GetMethod(() => NonVoidMethod());
                var returnType = methodInfo.ReturnType;
                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(methodInfo);

                _defaultForType = mock<IDefaultForType>();
                _defaultForType.stub(x => x.GetDefaultFor(returnType)).Return(_expectedDefault);
            }

            public override ReturnDefaultForReturnTypeHandler CreateSubjectUnderTest()
            {
                return new ReturnDefaultForReturnTypeHandler(_defaultForType);
            }

            private object NonVoidMethod() { return null; }
        }
    }
}