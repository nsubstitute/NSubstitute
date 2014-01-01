using System;
using NSubstitute.Routing;

namespace NSubstitute.Core
{
    public class WhenCalled<T>
    {
        private readonly T _substitute;
        private readonly Action<T> _call;
        private readonly MatchArgs _matchArgs;
        private readonly ICallRouter _callRouter;
        private readonly IRouteFactory _routeFactory;

        public WhenCalled(ISubstitutionContext context, T substitute, Action<T> call, MatchArgs matchArgs)
        {
            _substitute = substitute;
            _call = call;
            _matchArgs = matchArgs;
            _callRouter = context.GetCallRouterFor(substitute);
            _routeFactory = context.GetRouteFactory();
        }

        /// <summary>
        /// Perform this action when called.
        /// </summary>
        /// <param name="callbackWithArguments"></param>
        public void Do(Action<CallInfo> callbackWithArguments)
        {
            _callRouter.SetRoute(x => _routeFactory.DoWhenCalled(x, callbackWithArguments, _matchArgs));
            _call(_substitute);
        }

        /// <summary>
        /// Do not call the base implementation on future calls. For us with partial substitutes.
        /// </summary>
        public void DoNotCallBase()
        {
            _callRouter.SetRoute(x => _routeFactory.DoNotCallBase(x, _matchArgs));
            _call(_substitute);
        }
    }
}