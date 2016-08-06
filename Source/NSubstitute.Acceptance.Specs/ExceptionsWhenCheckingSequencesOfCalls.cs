using System;
using NSubstitute.Exceptions;
using NSubstitute.Experimental;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class ExceptionsWhenCheckingSequencesOfCalls
    {
        public interface IFoo
        {
            void Bar();
            void Zap();
        }

        public interface IBaz
        {
            void Gloop(int i);
            void Slop(object o, string s);
        }

        public interface IBar
        {
            int Huh();
        }

        [Test]
        public void When_missing_a_call()
        {
            var sub = Substitute.For<IFoo>();
            sub.Bar();

            Action action = () =>
                            {
                                sub.Bar();
                                sub.Zap();
                            };
            ExpectMessageFromQuery(action, @"
Expected to receive these calls in order:

    Bar()
    Zap()

Actually received matching calls in this order:

    Bar()");
        }

        [Test]
        public void When_non_matching_args()
        {
            var anObject = new object();
            var sub = Substitute.For<IBaz>();
            sub.Gloop(2);
            sub.Slop(anObject, "hello");
            sub.Gloop(3);

            Action action = () =>
                            {
                                sub.Gloop(2);
                                sub.Slop(anObject, "hi");
                                sub.Gloop(123);
                            };
            ExpectMessageFromQuery(action, @"Expected to receive these calls in order:

    Gloop(2)
    Slop(Object, ""hi"")
    Gloop(123)

Actually received matching calls in this order:

    Gloop(2)");
        }

        [Test]
        public void When_extra_call()
        {
            var sub = Substitute.For<IFoo>();
            sub.Bar();
            sub.Bar();
            sub.Zap();

            Action action = () =>
                            {
                                sub.Bar();
                                sub.Zap();
                            };

            ExpectMessageFromQuery(action, @"Expected to receive these calls in order:

    Bar()
    Zap()

Actually received matching calls in this order:

    Bar()
    Bar()
    Zap()
");
        }

        [Test]
        public void When_checking_across_multiple_subs_with_same_type()
        {
            var sub0 = Substitute.For<IFoo>();
            var sub1 = Substitute.For<IFoo>();

            sub0.Bar();
            sub1.Bar();
            sub0.Zap();
            sub1.Zap();

            Action action = () =>
                            {
                                sub0.Bar();
                                sub1.Bar();
                                sub1.Bar();
                                sub1.Zap();
                                sub0.Zap();
                            };

            ExpectMessageFromQuery(action, @"Expected to receive these calls in order:

    1@IFoo.Bar()
    2@IFoo.Bar()
    2@IFoo.Bar()
    2@IFoo.Zap()
    1@IFoo.Zap()

Actually received matching calls in this order:

    1@IFoo.Bar()
    2@IFoo.Bar()
    1@IFoo.Zap()
    2@IFoo.Zap()");
        }

        [Test]
        public void When_checking_across_multiple_subs_including_delegates()
        {
            var sub0 = Substitute.For<IFoo>();
            var sub1 = Substitute.For<Func<int, string>>();
            var sub2 = Substitute.For<IFoo>();

            sub0.Bar();
            sub1(2);
            sub0.Zap();
            sub2.Zap();

            Action action = () =>
                            {
                                sub0.Bar();
                                sub1(2);
                                sub2.Zap();
                                sub0.Zap();
                            };

            ExpectMessageFromQuery(action, @"Expected to receive these calls in order:

    1@IFoo.Bar()
    2@Func<Int32, String>.Invoke(2)
    3@IFoo.Zap()
    1@IFoo.Zap()

Actually received matching calls in this order:

    1@IFoo.Bar()
    2@Func<Int32, String>.Invoke(2)
    1@IFoo.Zap()
    3@IFoo.Zap()");
        }

        [Test]
        public void Do_not_number_instances_when_types_are_sufficient_to_identify_calls()
        {
            var a = Substitute.For<IFoo>();
            var b = Substitute.For<IBaz>();
            var c = Substitute.For<IBar>();

            a.Zap();
            c.Huh();
            b.Gloop(1);
            a.Bar();

            Action query = () =>
                             {
                                 a.Zap();
                                 b.Gloop(1);
                                 c.Huh();
                                 a.Bar();
                             };

            ExpectMessageFromQuery(query, @"Expected to receive these calls in order:

    IFoo.Zap()
    IBaz.Gloop(1)
    IBar.Huh()
    IFoo.Bar()

Actually received matching calls in this order:

    IFoo.Zap()
    IBar.Huh()
    IBaz.Gloop(1)
    IFoo.Bar()");
        }

        private void ExpectMessageFromQuery(Action query, string message)
        {
            var actualMessage = Assert.Throws<CallSequenceNotFoundException>(() => Received.InOrder(query)).Message;

            Assert.That(TrimAndFixLineEndings(actualMessage), Is.StringStarting(TrimAndFixLineEndings(message)));
        }

        private string TrimAndFixLineEndings(string s)
        {
            return s.Trim().Replace("\r\n", "\n");
        }
    }
}
