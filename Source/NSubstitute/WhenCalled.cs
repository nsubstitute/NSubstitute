using System;

namespace NSubstitute
{
    public class WhenCalled<T>
    {
        private readonly T _substitute;
        private readonly Action<T> _call;
        private ICallRouter _callRouter;

        public WhenCalled(ISubstitutionContext context, T substitute, Action<T> call)
        {
            _substitute = substitute;
            _call = call;
            _callRouter = context.GetCallRouterFor(substitute);
        }

        public void Do(Action<object[]> callbackWithArguments)
        {
            _callRouter.SetRoute<DoWhenCalledRoute>(callbackWithArguments);
            _call(_substitute);
        }

    }
}