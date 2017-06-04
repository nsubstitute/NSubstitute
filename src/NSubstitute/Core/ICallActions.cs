using System;

namespace NSubstitute.Core
{
    public interface ICallActions
    {
        void Add(ICallSpecification callSpecification, Action<CallInfo> action);
        void Add(ICallSpecification callSpec);
        void InvokeMatchingActions(ICall callInfo);
        void MoveActionsForSpecToNewSpec(ICallSpecification oldCallSpecification, ICallSpecification newCallSpecification);
		void Clear();
    }
}