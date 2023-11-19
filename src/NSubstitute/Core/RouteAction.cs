namespace NSubstitute.Core
{
    public class RouteAction
    {
        private static readonly RouteAction _continue = new(hasReturnValue: false, null);

        public bool HasReturnValue { get; }

        public object? ReturnValue { get; }

        public static RouteAction Continue() => _continue;
        public static RouteAction Return(object? value) => new(hasReturnValue: true, value);

        private RouteAction(bool hasReturnValue, object? returnValue)
        {
            HasReturnValue = hasReturnValue;
            ReturnValue = returnValue;
        }
    }
}
