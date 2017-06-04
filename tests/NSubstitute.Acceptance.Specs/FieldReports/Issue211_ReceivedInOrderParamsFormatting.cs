using System;
using System.Globalization;
using System.Text.RegularExpressions;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue211_ReceivedInOrderParamsFormatting
    {
        public interface IAmAnInterface
        {
            void ParamsCall(params int[] i);
            void ParamsCall(params string[] i);
        }

        [Test]
        public void TestFormattingForStandardReceived()
        {
            var sub = Substitute.For<IAmAnInterface>();

            sub.ParamsCall(1, 42);

            var ex = Assert.Throws<ReceivedCallsException>(() =>
                sub.Received().ParamsCall(1, 1)
            );

            //Show expected call:
            Assert.That(ex.Message, Is.StringContaining("ParamsCall(1, 1)"));
            //Show actual call:
            Assert.That(ex.Message, Is.StringContaining("ParamsCall(1, *42*)"));
        }

        [Test]
        public void TestFormattingReceivedInOrder()
        {
            var sub = Substitute.For<IAmAnInterface>();

            sub.ParamsCall(1, 1);
            sub.ParamsCall(1, 2);

            Action assertion = () =>
                Received.InOrder(() =>
                {
                    sub.ParamsCall(1, 2);
                    sub.ParamsCall(1, 1);
                });

            var ex = Assert.Throws<CallSequenceNotFoundException>(() => assertion());
            var expectedMessage = @"
                    Expected to receive these calls in order:

                        ParamsCall(1, 2)
                        ParamsCall(1, 1)

                    Actually received matching calls in this order:

                        ParamsCall(1, 1)
                        ParamsCall(1, 2)
                    ";

            ContainsExcludingWhitespace(ex.Message, expectedMessage);
        }

        static void ContainsExcludingWhitespace(string haystack, string needle)
        {
            var ws = new Regex(@"\s+");
            var spacifiedHaystack = ws.Replace(haystack, " ");
            var spacifiedNeedle = ws.Replace(needle, " ");

            Assert.That(spacifiedHaystack, Is.StringContaining(spacifiedNeedle));
        }
    }
}
