using System;
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

        public override void Context()
        {
            _callArguments = new[] {new object(), new object()};
            _call = mock<ICall>();
            _call.stub(x => x.GetArguments()).Return(_callArguments);
        }

        public override void Because()
        {
            _result = sut.Create(_call);
        }

        [Test]
        public void Should_create_info_with_arguments_from_call()
        {
            Assert.That(_result.GetArguments(), Is.SameAs(_callArguments));
        }

        public override CallInfoFactory CreateSubjectUnderTest()
        {
            return new CallInfoFactory();
        }
    }
}