using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallInfoSpecs
    {
        private static Argument CreateArg<T>(T value) { return new Argument(typeof(T), () => value, x => { }); }

        public class Simple_argument_types : ConcernFor<CallInfo>
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
                Assert.Throws<AmbiguousArgumentsException>(() => sut.Arg<int>());
            }

            [Test]
            public void Should_throw_exception_when_no_argument_match_by_type()
            {
                Assert.Throws<ArgumentNotFoundException>(() => sut.Arg<Guid>());
            }

            public override void Context()
            {
                _arguments = new[] { CreateArg(5), CreateArg("test"), CreateArg(1.234), CreateArg(10), CreateArg(new string[0]) };
            }

            public override CallInfo CreateSubjectUnderTest()
            {
                return new CallInfo(_arguments);
            }
        }

        public class Finding_arguments_by_type
        {
            [Test]
            public void Match_argument_by_declared_type_when_an_exact_match_is_found()
            {
                var sut = new CallInfo(new[] { CreateArg<object>("hello"), CreateArg("world") });

                Assert.That(sut.Arg<object>(), Is.EqualTo("hello"));
                Assert.That(sut.Arg<string>(), Is.EqualTo("world"));
            }

            [Test]
            public void Match_argument_by_actual_type_when_no_declared_type_match_is_found()
            {
                var sut = new CallInfo(new[] { CreateArg<object>(123), CreateArg<object>("hello") });

                Assert.That(sut.Arg<string>(), Is.EqualTo("hello"));
                Assert.That(sut.Arg<int>(), Is.EqualTo(123));
            }

            [Test]
            public void Match_argument_by_actual_type_when_no_declared_type_match_is_found_and_when_a_compatible_argument_is_provided()
            {
                var list = new List<int>();
                var sut = new CallInfo(new[] { CreateArg<object>("asdf"), CreateArg<object>(list) });
                Assert.That(sut.Arg<IEnumerable<int>>(), Is.SameAs(list));
            }

            [Test]
            public void Match_by_ref_type_arguments_when_argument_that_is_the_non_by_ref_type_is_provided()
            {
                const int expectedResult = 5;
                var sut = new CallInfo(new[] { CreateArg<object>("aasdf"), new ByRefArgument<int>(expectedResult) });
                Assert.That(sut.Arg<int>(), Is.EqualTo(expectedResult));
            }

            [Test]
            public void Match_by_ref_type_arguments_when_argument_compatbile_non_by_ref_type_is_provided()
            {
                var list = new List<int>();
                var sut = new CallInfo(new[] { CreateArg<object>("aasdf"), new ByRefArgument<List<int>>(list) });
                Assert.That(sut.Arg<IEnumerable<int>>(), Is.EqualTo(list));
            }

            [Test]
            public void Throw_when_there_is_no_declared_type_match_but_multiple_compatible_arguments()
            {
                var sut = new CallInfo(new[] { CreateArg<object>("a"), CreateArg<object>("b") });
                Assert.Throws<AmbiguousArgumentsException>(() => sut.Arg<string>());
            }
        }

        public class When_setting_argument_values
        {
            [Test]
            public void Throw_when_setting_argument_that_is_not_passed_by_ref()
            {
                var sut = new CallInfo(new[] { CreateArg(1) });

                Assert.Throws<ArgumentIsNotOutOrRefException>(() => sut[0] = 123);
            }

            [Test]
            public void Throw_when_setting_by_ref_argument_with_an_incompatible_value()
            {
                var sut = new CallInfo(new[] { new ByRefArgument<object>("needs a string") });

                Assert.Throws<ArgumentSetWithIncompatibleValueException>(() => sut[0] = new object());
            }
        }

        private class ByRefArgument<T> : Argument
        {
            /* Simulating how an T& argument (like Int32&) comes through to CallInfo */
            public ByRefArgument(T value) : base(typeof(T).MakeByRefType(), () => value, x => { }) { }
            public override Type ActualType { get { return Value.GetType().MakeByRefType(); } }
        }
    }
}