using NSubstitute.Exceptions;

namespace NSubstitute.Core
{
    /// <summary>
    /// Defines an internal interface
    /// that provides <see cref="ConfigureCall"/>
    /// validation.
    /// </summary>
    internal interface IConfigureCallValidator
    {
        /// <summary>
        /// Checks for spec overlap.
        /// </summary>
        /// <param name="spec">The spec.</param>
        void CheckForSpecOverlap(ICallSpecification spec);

        /// <summary>
        /// Checks the result is compatible with call.
        /// </summary>
        /// <param name="valueToReturn">The value to return.</param>
        /// <param name="spec">The spec.</param>
        void CheckResultIsCompatibleWithCall(IReturn valueToReturn, ICallSpecification spec);
    }

    /// <summary>
    /// Defines a class that implements <see cref="IConfigureCallValidator"/>.
    /// </summary>
    internal class ConfigureCallValidator : IConfigureCallValidator
    {
        private readonly ICallResults _configuredResults;

        public ConfigureCallValidator(ICallResults configuredResults)
        {
            _configuredResults = configuredResults;
        }

        public void CheckForSpecOverlap(ICallSpecification spec)
        {
            // TODO: Create partial copy to match arguments?   
            var anyArgsSpec = spec.CreateCopyThatMatchesAnyArguments();
            var result = _configuredResults.GetResultBySpecSimilarity(anyArgsSpec);
            var undoable = result as IUndoable;
            if (undoable != null) undoable.Undo();
        }

        public void CheckResultIsCompatibleWithCall(IReturn valueToReturn, ICallSpecification spec)
        {
            var requiredReturnType = spec.ReturnType();
            if (!valueToReturn.CanBeAssignedTo(requiredReturnType))
            {
                throw new CouldNotSetReturnDueToTypeMismatchException(valueToReturn.TypeOrNull(), spec.GetMethodInfo());
            }
        }
    }
}