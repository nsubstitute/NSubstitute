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

        [Test]
        [Pending]
        public void When_missing_a_call()
        {
            var sub = Substitute.For<IFoo>();
            sub.Bar();

            Action action = () =>
                            {
                                sub.Bar();
                                sub.Zap();
                            };
            ExpectMessageFromQuery(action, "!todo!");
        }

        [Test]
        [Pending]
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
            ExpectMessageFromQuery(action, "!todo!");
        }

        [Test]
        [Pending]
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

            ExpectMessageFromQuery(action, "!todo!");
        }

        [Test]
        [Pending]
        public void When_checking_across_multiple_subs()
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

            ExpectMessageFromQuery(action, "!todo!");
        }

        [Test]
        [Pending]
        public void When_checking_across_multiple_subs_including_delegates()
        {
            var sub0 = Substitute.For<IFoo>();
            var sub1 = Substitute.For<Func<string>>();
            var sub2 = Substitute.For<IFoo>();

            sub0.Bar();
            sub1();
            sub0.Zap();
            sub2.Zap();

            Action action = () =>
                            {
                                sub0.Bar();
                                sub1();
                                sub2.Zap();
                                sub0.Zap();
                            };

            ExpectMessageFromQuery(action, "!todo!");
        }

        private void ExpectMessageFromQuery(Action query, string message)
        {
            var actualMessage = Assert.Throws<CallSequenceNotFoundException>(() => Received.InOrder(query)).Message;
            Assert.That(actualMessage, Is.StringContaining(message));
        }
    }
}