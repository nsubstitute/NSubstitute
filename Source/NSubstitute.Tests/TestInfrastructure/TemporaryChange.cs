using System;

namespace NSubstitute.Tests.TestInfrastructure
{
    public class TemporaryChange<T> : ITemporaryChange
    {
        public T TemporaryValue { get; set; }
        public T OriginalValue { get; private set; }
        public Action<T> SetValue { get; set; }

        public TemporaryChange(T value)
        {
            OriginalValue = value;
        }

        public void SetNewValue()
        {
            SetValue(TemporaryValue);
        }

        public void RestoreOriginalValue()
        {
            SetValue(OriginalValue);
        }
    }
}