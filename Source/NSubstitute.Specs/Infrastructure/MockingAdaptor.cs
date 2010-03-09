using System;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace NSubstitute.Specs.TestInfrastructure
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

        public static void did_not_receive<T>(this T mock, Action<T> call)
        {
            mock.AssertWasNotCalled(call);    
        }

        public static IMethodOptions<object> stub<T>(this T mock, Action<T> call) where T : class
        {
            return mock.Stub(call);
        }
    }
}