using NSubstitute.Exceptions;

namespace NSubstitute.Core.Arguments;

public class ArgumentSpecificationFactory : IArgumentSpecificationFactory
{
    public IArgumentSpecification Create(object? argument, IParameterInfo parameterInfo,
        ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
    {
        return parameterInfo.IsParams
            ? CreateSpecFromParamsArg(argument, parameterInfo, suppliedArgumentSpecifications)
            : CreateSpecFromNonParamsArg(argument, parameterInfo, suppliedArgumentSpecifications);
    }

    private IArgumentSpecification CreateSpecFromNonParamsArg(object? argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
    {
        if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
        {
            return suppliedArgumentSpecifications.Dequeue();
        }

        bool isAmbiguousSpecificationPresent = suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType);
        if (!isAmbiguousSpecificationPresent || parameterInfo.IsOptional || parameterInfo.IsOut)
        {
            return new ArgumentSpecification(parameterInfo.ParameterType, new EqualsArgumentMatcher(argument));
        }

        throw new AmbiguousArgumentsException();
    }

    private IArgumentSpecification CreateSpecFromParamsArg(object? argument, IParameterInfo parameterInfo, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
    {
        // Next specification is for the whole params argument.
        if (suppliedArgumentSpecifications.IsNextFor(argument, parameterInfo.ParameterType))
        {
            return suppliedArgumentSpecifications.Dequeue();
        }

        // Check whether the specification ambiguity could happen.
        bool isAmbiguousSpecificationPresent = suppliedArgumentSpecifications.AnyFor(argument, parameterInfo.ParameterType);
        if (isAmbiguousSpecificationPresent)
        {
            throw new AmbiguousArgumentsException();
        }

        // User passed "null" as the params value.
        if (argument == null)
        {
            return new ArgumentSpecification(parameterInfo.ParameterType, new EqualsArgumentMatcher(null));
        }

        // User specified arguments using the native params syntax.
        var elementType = ParamsSupport.GetElementType(parameterInfo.ParameterType);
        var argumentValueSpecifications = UnwrapParamsArguments(ParamsSupport.UnwrapArgument(argument), elementType, suppliedArgumentSpecifications);
        return new ArgumentSpecification(parameterInfo.ParameterType, new ParamsContentsArgumentMatcher(argumentValueSpecifications));
    }

    private IEnumerable<IArgumentSpecification> UnwrapParamsArguments(IEnumerable<object?> args, Type paramsElementType, ISuppliedArgumentSpecifications suppliedArgumentSpecifications)
    {
        var fakeParameterInfo = new ParameterInfoFromType(paramsElementType);
        var result = new List<IArgumentSpecification>();
        foreach (var arg in args)
        {
            try
            {
                result.Add(CreateSpecFromNonParamsArg(arg, fakeParameterInfo, suppliedArgumentSpecifications));
            }
            catch (AmbiguousArgumentsException ex)
            {
                ex.Data[AmbiguousArgumentsException.NonReportedResolvedSpecificationsKey] = result;
                throw;
            }
        }

        return result;
    }

    private class ParameterInfoFromType(Type parameterType) : IParameterInfo
    {
        public Type ParameterType { get; } = parameterType;

        public bool IsParams => false;

        public bool IsOptional => false;

        public bool IsOut => false;
    }
}