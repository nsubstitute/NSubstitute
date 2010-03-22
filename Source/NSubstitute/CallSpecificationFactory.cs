using System.Collections.Generic;

namespace NSubstitute
{
    public class CallSpecificationFactory : ICallSpecificationFactory
    {
        public ICallSpecification Create(ICall call)
        {
            var result = new CallSpecification(call.GetMethodInfo());
            foreach (var argument in call.GetArguments())
            {
                var specifiedArgument = argument;
                result.ArgumentMatchers.Add(new ArgumentMatcher(o => EqualityComparer<object>.Default.Equals(o, specifiedArgument)));
            }
            return result;
        }
    }
}