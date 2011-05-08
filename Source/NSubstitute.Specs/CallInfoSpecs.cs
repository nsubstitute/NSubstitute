using System;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallInfoSpecs : ConcernFor<CallInfo>
    {
        private Argument[] _arguments;

        [Test]
        public void Can_get_arguments()
        {
            Assert.That(sut.Args(), Is.EqualTo(_arguments.Select(x => x.Value).ToArray()));
        }

        [Test]
        public void Can_get_argument_types()
        {
            Assert.That(sut.ArgTypes(), Is.EqualTo(_arguments.Select(x => x.DeclaredType).ToArray()));
        }

        [Test]
        public void Can_get_arguments_by_index()
        {
            for (int i = 0; i < _arguments.Length; i++)
            {
                Assert.That(sut[i], Is.EqualTo(_arguments[i].Value));
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
            _arguments = new[] {CreateArg(5), CreateArg("test"), CreateArg(1.234), CreateArg(10), CreateArg(new string[0])};
        }

        private Argument CreateArg<T>(T value) { return new Argument(typeof (T), value); }

        public override CallInfo CreateSubjectUnderTest()
        {
            return new CallInfo(_arguments);
        }
    }
}