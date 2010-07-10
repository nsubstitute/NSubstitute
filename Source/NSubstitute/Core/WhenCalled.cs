using System;
using NSubstitute.Routes;

namespace NSubstitute.Core
{
    public class WhenCalled<T>
    {
        private readonly T _substitute;
        private readonly Action<T> _call;
        private readonly MatchArgs _matchArgs;
        private ICallRouter _callRouter;

        public WhenCalled(ISubstitutionContext context, T substitute, Action<T> call, MatchArgs matchArgs)
        {
            _substitute = substitute;
            _call = call;
            _matchArgs = matchArgs;
            _callRouter = context.GetCallRouterFor(substitute);
        }

        public void Do(Action<CallInfo> callbackWithArguments)
        {
            _callRouter.SetRoute<DoWhenCalledRoute>(callbackWithArguments, _matchArgs);
            _call(_substitute);
        }

    }
}