using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public abstract class ArgumentsFormatterSpecs
    {
        public class When_formatting_arguments : ConcernFor<ArgumentsFormatter>
        {
            private IArgumentFormatter _argumentFormatter;
            private readonly object[] _arguments = new [] { new object(), new object(), new object() };
            private readonly int[] _argumentsToHighlight = new[] { 0, 2 };
            private string _result;

            public override void Context()
            {
                _argumentFormatter = mock<IArgumentFormatter>();
                _argumentFormatter.stub(x => x.Format(_arguments[0])).Return("0");
                _argumentFormatter.stub(x => x.Format(_arguments[1])).Return("1");
                _argumentFormatter.stub(x => x.Format(_arguments[2])).Return("2");
            }

            public override void Because()
            {
                _result = sut.Format(_arguments, _argumentsToHighlight);
            }

            [Test]
            public void Should_format_each_argument_and_highlight_required_arguments_using_asterisks()
            {
                Assert.That(_result, Is.EqualTo("*0*, 1, *2*")); 
            }

            public override ArgumentsFormatter CreateSubjectUnderTest()
            {
                return new ArgumentsFormatter(_argumentFormatter);
            }
        }
    }
}