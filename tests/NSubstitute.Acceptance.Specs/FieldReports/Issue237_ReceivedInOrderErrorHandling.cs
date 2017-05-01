using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue237_ReceivedInOrderErrorHandling
    {
        public interface IAmAnInterface
        {
            void MethodA(int a, int b);
            void MethodB(int a, int b);

            int MethodC();
        }

        [Test]
        public void AnExceptionIsReceivedWhenExpected()
        {
            Assert.Throws<Exception>(() =>
                Received.InOrder(() =>
                {
                    throw new Exception("An Exception!");
                }));
        }

        [Test]
        public void MethodCallsAreReceivedInOrder()
        {
            IAmAnInterface _interface = Substitute.For<IAmAnInterface>();
            _interface.MethodA(1, 2);
            _interface.MethodB(1, 2);
            Received.InOrder(() =>
            {
                _interface.MethodA(1, 2);
                _interface.MethodB(1, 2);
            });
        }

        [Test]
        public void AfterTheFailingTestIsRunWhenTheSuccessfulTestIsRunTheSuccessfulTestShouldSucceed()
        {
            try
            {
                AnExceptionIsReceivedWhenExpected();
            }
            catch (Exception)
            {
                // suppress exception from first test
            }
            MethodCallsAreReceivedInOrder();
        }

        [Test]
        public void AfterTheFailingTestIsRunIShouldBeAbleToConfigureASubstitute()
        {
            try
            {
                AnExceptionIsReceivedWhenExpected();
            }
            catch (Exception)
            {
                // suppress failure of test A
            }
            var foo = Substitute.For<IAmAnInterface>();
            foo.MethodC().Returns(1);
        }
    }
}
