using System.Collections.Generic;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Arguments
{
    public abstract class ArgumentsFormatterSpecs
    {
        public class When_formatting_arguments : ConcernFor<ArgumentsFormatter>
        {
            private string _result;
            private List<IArgumentFormatInfo> _argumentFormatInfos;
            private IArgumentFormatter _argumentFormatter;


            public override void Context()
            {
                _argumentFormatter = mock<IArgumentFormatter>();
                _argumentFormatInfos = new List<IArgumentFormatInfo>();
                _argumentFormatInfos.Add(CreateArgumentFormatInfoFor("*0*"));
                _argumentFormatInfos.Add(CreateArgumentFormatInfoFor("1"));
                _argumentFormatInfos.Add(CreateArgumentFormatInfoFor("*2*"));
            }

            private IArgumentFormatInfo CreateArgumentFormatInfoFor(string format)
            {
                IArgumentFormatInfo argumentFormatInfo = mock<IArgumentFormatInfo>();
                argumentFormatInfo.stub(x => x.Format(_argumentFormatter)).Return(format);

                return argumentFormatInfo;
            }

            public override void Because()
            {
                _result = sut.Format(_argumentFormatInfos);
            }

            [Test]
            public void Should_format_and_join_each_argument()
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