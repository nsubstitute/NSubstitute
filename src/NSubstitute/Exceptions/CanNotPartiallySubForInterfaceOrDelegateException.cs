namespace NSubstitute.Exceptions;

public class CanNotPartiallySubForInterfaceOrDelegateException(Type type) : SubstituteException(DescribeProblem(type))
{
    private static string DescribeProblem(Type type)
    {
        return string.Format("Can only substitute for parts of classes, not interfaces or delegates. "
                            + "Try `Substitute.For<{0}> instead.", type.Name);
    }
}
