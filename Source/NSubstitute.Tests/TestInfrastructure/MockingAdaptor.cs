using System;
using Rhino.Mocks;

namespace NSubstitute.Tests.TestInfrastructure
{
    public static class MockingAdaptor
    {
        public static T Create<T>() where T : class
        {
            return MockRepository.GenerateStub<T>();
        }

        public static void received<T>(this T mock, Action<T> callReceived)
        {
            mock.AssertWasCalled(callReceived);
        }
    }
}