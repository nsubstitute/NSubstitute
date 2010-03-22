using System;

namespace NSubstitute.Specs.Infrastructure
{
    public class TemporaryChangeViaBuilder<T>
    {
        TemporaryChange<T> _change;

        public TemporaryChangeViaBuilder(TemporaryChange<T> change)
        {
            _change = change;
        }

        public void via(Action<T> setValue)
        {
            _change.SetValue = setValue;
            _change.SetNewValue();
        }
    }
}