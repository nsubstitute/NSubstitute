/*
    This is what I want to write to unit test CastleInvocationMapper.
    However, because Castle is IL-merged into NSubstitute privately I can't.
    I tried assembly:InternalsVisibleTo, but that didn't work, neither did referencing
    Castle.Core in this project. I suspect the only way it's possible is if there is a
    special build of NSubstitute where the IL-merge is public for the test project to test.
    That obviously has problems of it's own given the DLL being tested isn't then the one
    eventually being shipped...
    I have to leave this commented out since I can't even get it compiling.

using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Proxies.CastleDynamicProxy
{
    public class CastleInvocationMapperSpecs
    {
        public class When_mapping_a_castle_invocation_to_a_call : ConcernFor<CastleInvocationMapper>
        {
            private const int OriginalReturnValue = 5;
            private IInvocation _invocation;
            private ICall _result;

            public override void Context()
            {
                _invocation = mock<IInvocation>();
                // Stub out the Proceed call which in turn calls the original method
                //  and sets the invocation return value
                _invocation.stub(i => i.Proceed())
                    .WhenCalled(m => { _invocation.ReturnValue = OriginalReturnValue; });
            }

            public override CastleInvocationMapper CreateSubjectUnderTest()
            {
                return new CastleInvocationMapper();
            }

            public override void Because()
            {
                _result = sut.Map(_invocation);
            }

            [Test]
            public void Should_correctly_map_original_method_call()
            {
                var originalResult = _result.CallOriginalMethod();
                Assert.That(originalResult, Is.EqualTo(OriginalReturnValue));
            }
        }
    }
}
*/
