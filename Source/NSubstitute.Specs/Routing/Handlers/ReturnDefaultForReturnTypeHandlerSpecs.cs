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
            object _result;

            [Test]
            public void Should_return_default_for_type()
            {
                Assert.That(_result, Is.SameAs(_expectedDefault));

            }
            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                var returnType = typeof(object);
                var methodInfo = mock<MethodInfo>();
                methodInfo.stub(x => x.ReturnType).Return(returnType);
                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(methodInfo);

                _defaultForType = mock<IDefaultForType>();
                _defaultForType.stub(x => x.GetDefaultFor(returnType)).Return(_expectedDefault);
            }

            public override ReturnDefaultForReturnTypeHandler CreateSubjectUnderTest()
            {
                return new ReturnDefaultForReturnTypeHandler(_defaultForType);
            }
        }
    }
}