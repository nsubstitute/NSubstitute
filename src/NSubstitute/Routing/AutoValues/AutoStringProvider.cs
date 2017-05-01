using System;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoStringProvider : IAutoValueProvider
    {
        public bool CanProvideValueFor(Type type)
        {
            return type == typeof(string); 
        }

        public object GetValue(Type type)
        {
            return string.Empty;
        }
    }
}