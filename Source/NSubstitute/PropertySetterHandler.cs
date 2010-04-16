using System.Linq;

namespace NSubstitute
{
    public class PropertySetterHandler : ICallHandler
    {
        private readonly IReflectionHelper _reflectionHelper;
        readonly IResultSetter _resultSetter;

        public PropertySetterHandler(IReflectionHelper reflectionHelper, IResultSetter resultSetter)
        {
            _reflectionHelper = reflectionHelper;
            _resultSetter = resultSetter;
        }

        public object Handle(ICall call)
        {
            if (_reflectionHelper.IsCallToSetAReadWriteProperty(call))
            {
                var callToPropertyGetter = _reflectionHelper.CreateCallToPropertyGetterFromSetterCall(call);
                var valueBeingSetOnProperty = call.GetArguments().First();
                _resultSetter.SetResultForCall(callToPropertyGetter, valueBeingSetOnProperty);
            }
            return null;
        }
    }
}