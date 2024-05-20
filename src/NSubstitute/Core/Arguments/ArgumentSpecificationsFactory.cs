using NSubstitute.Exceptions;
using System.Reflection;

namespace NSubstitute.Core.Arguments;

public class ArgumentSpecificationsFactory(
    IArgumentSpecificationFactory argumentSpecificationFactory,
    ISuppliedArgumentSpecificationsFactory suppliedArgumentSpecificationsFactory) : IArgumentSpecificationsFactory
{
    public IEnumerable<IArgumentSpecification> Create(IList<IArgumentSpecification> argumentSpecs, object?[] arguments, IParameterInfo[] parameterInfos, MethodInfo methodInfo, MatchArgs matchArgs)
    {
        var suppliedArgumentSpecifications = suppliedArgumentSpecificationsFactory.Create(argumentSpecs);

        var result = new List<IArgumentSpecification>();
        for (var i = 0; i < arguments.Length; i++)
        {
            var arg = arguments[i];
            var paramInfo = parameterInfos[i];

            try
            {
                result.Add(argumentSpecificationFactory.Create(arg, paramInfo, suppliedArgumentSpecifications));
            }
            catch (AmbiguousArgumentsException ex) when (ex.ContainsDefaultMessage)
            {
                IEnumerable<IArgumentSpecification> alreadyResolvedSpecs = result;
                if (ex.Data[AmbiguousArgumentsException.NonReportedResolvedSpecificationsKey] is IEnumerable<IArgumentSpecification> additional)
                {
                    alreadyResolvedSpecs = alreadyResolvedSpecs.Concat(additional);
                }

                throw new AmbiguousArgumentsException(methodInfo, arguments, alreadyResolvedSpecs, argumentSpecs);
            }
        }

        var remainingArgumentSpecifications = suppliedArgumentSpecifications.DequeueRemaining();
        if (remainingArgumentSpecifications.Any())
        {
            throw new RedundantArgumentMatcherException(remainingArgumentSpecifications, argumentSpecs);
        }

        return matchArgs == MatchArgs.Any
            ? ConvertToMatchAnyValue(result)
            : result;
    }

    private static IEnumerable<IArgumentSpecification> ConvertToMatchAnyValue(IEnumerable<IArgumentSpecification> specs)
    {
        return specs.Select(x => x.CreateCopyMatchingAnyArgOfType(x.ForType)).ToArray();
    }
}