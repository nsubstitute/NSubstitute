using System;
using NSubstitute.Routes;

namespace NSubstitute.Core
{
    public class WhenCalled<T>
    {
        private readonly T _substitute;
        private readonly Action<T> _call;
        private readonly bool _matchAnyArguments;
        private ICallRouter _callRouter;

        public WhenCalled(ISubstitutionContext context, T substitute, Action<T> call, bool matchAnyArguments)
        {
            _substitute = substitute;
            _call = call;
            _matchAnyArguments = matchAnyArguments;
            _callRouter = context.GetCallRouterFor(substitute);
        }

        public void Do(Action<CallInfo> callbackWithArguments)
        {
            _callRouter.SetRoute<DoWhenCalledRoute>(callbackWithArguments, _matchAnyArguments);
            _call(_substitute);
        }

    }
}