using System;
using System.Text.RegularExpressions;
using NSubstitute.Core;
using NSubstitute.Exceptions;
using NSubstitute.Experimental;
using NUnit.Framework;
using System.Linq;

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

            ExpectMessageFromQuery(action, @"Expected to receive these calls in order:
    IFoo#1.Bar()
    IFoo#2.Bar()
    IFoo#2.Bar()
    IFoo#2.Zap()
    IFoo#1.Zap()
Actually received matching calls in this order:
    IFoo#1.Bar()
    IFoo#2.Bar()
    IFoo#1.Zap()
    IFoo#2.Zap()");
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
    IFoo#1.Bar()
    Func<Int32, String>#1.Invoke(2)
    IFoo#2.Zap()
    IFoo#1.Zap()
Actually received matching calls in this order:
    IFoo#1.Bar()
    Func<Int32, String>#1.Invoke(2)
    IFoo#1.Zap()
    IFoo#2.Zap()");
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