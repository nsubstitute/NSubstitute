using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.Arguments
{
    public class SuppliedArgumentSpecifications : ISuppliedArgumentSpecifications
    {
        private readonly IDefaultChecker _defaultChecker;
        private readonly Queue<IArgumentSpecification> _queue;
        private readonly List<IArgumentSpecification> _list;

        public SuppliedArgumentSpecifications(IDefaultChecker defaultChecker, IEnumerable<IArgumentSpecification> argumentSpecifications)
        {
            _defaultChecker = defaultChecker;
            _list = new List<IArgumentSpecification>(argumentSpecifications);
            _queue = new Queue<IArgumentSpecification>(_list);
        }

        public bool AnyFor(object argument, Type argumentType)
        {
            return _list.Any(x => DoesArgSpecLookLikeItCouldBeForThisArgumentAndType(x, argument, argumentType));
        }

        public bool IsNextFor(object argument, Type argumentType)
        {
            if (_queue.Count <= 0) { return false; }
            var nextArgSpec = _queue.Peek();
            return DoesArgSpecLookLikeItCouldBeForThisArgumentAndType(nextArgSpec, argument, argumentType);
        }

        public IArgumentSpecification Dequeue()
        {
            return _queue.Dequeue();
        }

        public IEnumerable<IArgumentSpecification> DequeueRemaining()
        {
            var result = _queue.ToArray();
            _queue.Clear();
            return result;
        }

        private bool DoesArgSpecLookLikeItCouldBeForThisArgumentAndType(IArgumentSpecification argSpec, object argument, Type argumentType)
        {
            var typeArgSpecIsFor = argSpec.ForType;
            return AreTypesCompatible(argumentType, typeArgSpecIsFor) && IsProvidedArgumentTheOneWeWouldGetUsingAnArgSpecForThisType(argument, typeArgSpecIsFor);
        }

        private bool IsProvidedArgumentTheOneWeWouldGetUsingAnArgSpecForThisType(object argument, Type typeArgSpecIsFor)
        {
            return _defaultChecker.IsDefault(argument, typeArgSpecIsFor);
        }

        private bool AreTypesCompatible(Type argumentType, Type typeArgSpecIsFor)
        {
            return argumentType.IsAssignableFrom(typeArgSpecIsFor) ||
                (argumentType.IsByRef && !typeArgSpecIsFor.IsByRef && argumentType.IsAssignableFrom(typeArgSpecIsFor.MakeByRefType()));
        }
    }
}