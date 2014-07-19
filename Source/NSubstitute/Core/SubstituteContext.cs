using System;
using System.Collections.Generic;
using System.Linq;

namespace NSubstitute.Core
{
    public class SubstituteContext : ISubstituteContext
    {
        readonly RobustThreadLocal<IList<Func<ICall, RouteAction>>> _customCallHandlers = new RobustThreadLocal<IList<Func<ICall, RouteAction>>>(() => new List<Func<ICall, RouteAction>>());

        public Func<ICall, RouteAction>[] CustomCallHandlers { get { return _customCallHandlers.Value.ToArray(); } }

        public void EnqueueCustomCallHandler(Func<ICall, RouteAction> customCallHandler)
        {
            _customCallHandlers.Value.Add(customCallHandler);
        }
    }
}