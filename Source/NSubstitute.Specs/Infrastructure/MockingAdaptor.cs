using System;
using System.Linq.Expressions;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace NSubstitute.Specs.Infrastructure
{
    public static class It
    {
        public static T Is<T>(T value) { return Arg<T>.Is.Equal(value); }
        public static T IsAny<T>() { return Arg<T>.Is.Anything; }
        public static T Matches<T>(Expression<Predicate<T>> expression) { return Arg<T>.Matches(expression); }
    }

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

        public static void did_not_receive_with_any_args<T>(this T mock, Action<T> call)
        {
            mock.AssertWasNotCalled(call, options => options.IgnoreArguments());
        }

        public static IMethodOptions<object> stub<T>(this T mock, Action<T> call) where T : class
        {
            return mock.Stub(call);
        }

        public static IMethodOptions<R> stub<T, R>(this T mock, Func<T, R> call) where T : class
        {
            Function<T, R> function = t => call(t);
            return mock.Stub(function);
        }
    }
}