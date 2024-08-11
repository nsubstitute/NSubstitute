namespace NSubstitute.Exceptions;

public abstract class TypeForwardingException(string message) : SubstituteException(message)
{
}

public sealed class CanNotForwardCallsToClassNotImplementingInterfaceException(Type type) : TypeForwardingException(DescribeProblem(type))
{
    private static string DescribeProblem(Type type)
    {
        return string.Format("The provided class '{0}' doesn't implement all requested interfaces. ", type.Name);
    }
}

public sealed class CanNotForwardCallsToAbstractClassException(Type type) : TypeForwardingException(DescribeProblem(type))
{
    private static string DescribeProblem(Type type)
    {
        return string.Format("The provided class '{0}' is abstract. ", type.Name);
    }
}
