using System;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.DelegateProxy
{
    public abstract class DelegateCallSpecs : ConcernFor<DelegateCall>
    {
        protected IParameterInfo[] _parameterInfos;
        protected Type _returnType;
        protected ICallRouter _callRouter;

        public override void Context()
        {
            _callRouter = mock<ICallRouter>();
            _parameterInfos = new IParameterInfo[0];
        }

        public override DelegateCall CreateSubjectUnderTest()
        {
            return new DelegateCall(_callRouter, _returnType, _parameterInfos);
        }

        [Test]
        public void CallRouter_is_called_on_Invoke()
        {
            sut.Invoke(new object[0]);

            _callRouter.Received().Route(Arg.Any<ICall>());
        }

        public class When_return_type_is_string : DelegateCallSpecs
        {
            public override void Context()
            {
                base.Context();
                _returnType = typeof(string);
            }

            [Test]
            public void DelegateCallInvoke_should_have_string_return_type()
            {
                MethodInfo methodInfo = sut.DelegateCallInvoke;

                Assert.That(methodInfo.ReturnType, Is.EqualTo(_returnType));
            }
        }

        public class When_return_type_is_void : DelegateCallSpecs
        {
            public override void Context()
            {
                base.Context();
                _returnType = typeof(void);
            }

            [Test]
            public void DelegateCallInvoke_should_have_object_return_type()
            {
                MethodInfo methodInfo = sut.DelegateCallInvoke;

                Assert.That(methodInfo.ReturnType, Is.EqualTo(typeof(object)));
            }
        }
    }
}
