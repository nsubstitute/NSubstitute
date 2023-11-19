using System;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallInfoFactorySpecs : ConcernFor<CallInfoFactory>
    {
        private ICall _call;
        private CallInfo _result;
        private object[] _callArguments;
        private Type[] _callArgumentTypes;

        public override void Context()
        {
            _callArguments = new[] {new object(), new object()};
            _callArgumentTypes = new[] { typeof(string), typeof(int) };
            _call = mock<ICall>();
            _call.stub(x => x.GetArguments()).Return(_callArguments);
            _call.stub(x => x.GetParameterInfos()).Return(_callArgumentTypes.Select(x => StubParameterInfo(x)).ToArray());
        }

        public override void Because()
        {
            _result = sut.Create(_call);
        }

        [Test]
        public void Should_create_info_with_arguments_from_call()
        {
            Assert.That(_result.Args(), Is.EqualTo(_callArguments));
        }

        [Test]
        public void Should_create_info_with_argument_types_from_call()
        {
            Assert.That(_result.ArgTypes(), Is.EqualTo(_callArgumentTypes));
        }

        public override CallInfoFactory CreateSubjectUnderTest()
        {
            return new CallInfoFactory();
        }

        IParameterInfo StubParameterInfo(Type parameterType)
        {
            var parameterInfo = mock<IParameterInfo>();
            parameterInfo.stub(x => x.ParameterType).Return(parameterType);
            return parameterInfo;
        }
    }
}