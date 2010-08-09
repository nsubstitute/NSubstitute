using System;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoArrayProvider : IAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type.IsArray;
        }

        public object GetValue(Type type)
        {
            return Array.CreateInstance(type.GetElementType(), 0);
        }
    }
}