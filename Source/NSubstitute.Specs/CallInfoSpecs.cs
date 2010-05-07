using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallInfoSpecs : ConcernFor<CallInfo>
    {
        private object[] _arguments;

        [Test]
        public void Can_get_arguments()
        {
            Assert.That(sut.Args(), Is.EqualTo(_arguments));
        }

        [Test]
        public void Can_get_arguments_by_index()
        {
            for (int i = 0; i < _arguments.Length; i++)
            {
                Assert.That(sut[i], Is.EqualTo(_arguments[i]));
            }
        }

        [Test]
        public void Can_get_argument_by_type()
        {
            var stringArgument = sut.Arg<string>();
            Assert.That(stringArgument, Is.EqualTo("test"));
        }

        [Test]
        public void Throw_exception_when_ambiguous_argument_match_by_type()
        {
            Assert.Throws<AmbiguousArgumentsException>(
                () => sut.Arg<int>()
                );
        }

        [Test]
        public void Should_throw_exception_when_no_argument_match_by_type()
        {
            Assert.Throws<ArgumentNotFoundException>(
                () => sut.Arg<Guid>()
            );
        }

        public override void Context()
        {
            _arguments = new object[] {5, "test", 1.234, 10, new string[0]};
        }


        public override CallInfo CreateSubjectUnderTest()
        {
            return new CallInfo(_arguments);
        }
    }
}