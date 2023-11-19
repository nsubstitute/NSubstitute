using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public abstract class ReturnValueFromFuncSpec
    {
        public class When_provided_a_func_to_return_a_value_from : ConcernFor<ReturnValueFromFunc<string>>
        {
            const string ValueToReturn = "Hello World";
            Func<CallInfo, string> _func;
            CallInfo _callInfo;
            object _result;

            public override void Context()
            {
                 _callInfo = new CallInfo(new Argument[0]);
                _func = mock<Func<CallInfo, string>>();
                _func.stub(x => x(_callInfo)).Return(ValueToReturn);
            }

            public override void Because()
            {
                _result = sut.ReturnFor(_callInfo);
            }

            [Test]
            public void Should_return_the_value_from_the_func()
            {
                Assert.That(_result, Is.SameAs(ValueToReturn));
            }

            public override ReturnValueFromFunc<string> CreateSubjectUnderTest()
            {
                return new ReturnValueFromFunc<string>(_func);
            }
        }

        public class When_passing_null_as_the_func_to_return_a_reference_type_value_from : ConcernFor<ReturnValueFromFunc<string>>
        {
            [Test]
            public void Should_return_null()
            {
                Assert.That(sut.ReturnFor(new CallInfo(new Argument[0])), Is.Null);
            }

            public override ReturnValueFromFunc<string> CreateSubjectUnderTest()
            {
                return new ReturnValueFromFunc<string>(null);
            }
        }

        public class When_passing_null_as_the_func_to_return_a_value_type_value_from : StaticConcern
        {
            [Test]
            public void Should_throw_exception()
            {
                Assert.That(() => new ReturnValueFromFunc<int>(null), Throws.TypeOf<CannotReturnNullForValueType>());
            }
        }
    }
}