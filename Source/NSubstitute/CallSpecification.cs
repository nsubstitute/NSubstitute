using System.Collections.Generic;
using System.Reflection;

namespace NSubstitute
{
    public class CallSpecification: ICallSpecification
    {
        readonly List<IArgumentSpecification> _argumentSpecifications;
        public MethodInfo MethodInfo { get; private set; }

        public CallSpecification(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            _argumentSpecifications = new List<IArgumentSpecification>();
        }

        public IList<IArgumentSpecification> ArgumentSpecifications
        {
            get { return _argumentSpecifications; }
        }
    }
}