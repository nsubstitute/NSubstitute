namespace NSubstitute.Core
{
    public class RouteAction
    {
        private readonly object _returnValue;
        private readonly bool _hasReturnValue;

        public bool HasReturnValue { get { return _hasReturnValue; } }
        public object ReturnValue { get { return _returnValue; } }

        private static readonly RouteAction _continue = new RouteAction();
        public static RouteAction Continue() { return _continue; }
        public static RouteAction Return(object value) { return new RouteAction(value); }

        private RouteAction()
        {
            _hasReturnValue = false;
        }

        private RouteAction(object returnValue)
        {
            _returnValue = returnValue;
            _hasReturnValue = true;
        }
    }
}
