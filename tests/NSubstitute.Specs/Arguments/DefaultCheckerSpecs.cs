using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public class DefaultCheckerSpecs
    {
        public class When_checking_default_value : ConcernFor<DefaultChecker>
        {
            private IDefaultForType _defaultForType;
            private const int DefaultIntValue = 7;

            [Test]
            public void Should_return_true_for_default()
            {
                Assert.That(sut.IsDefault(DefaultIntValue, DefaultIntValue.GetType()), Is.True);
            }

            [Test]
            public void Should_return_false_for_non_default()
            {
                Assert.That(sut.IsDefault(0, DefaultIntValue.GetType()), Is.False);
            }

            public override void Context()
            {
                _defaultForType = mock<IDefaultForType>();
                _defaultForType.stub(x => x.GetDefaultFor(DefaultIntValue.GetType())).Return(DefaultIntValue);
            }

            public override DefaultChecker CreateSubjectUnderTest()
            {
                return new DefaultChecker(_defaultForType);
            }
        }
    }
}