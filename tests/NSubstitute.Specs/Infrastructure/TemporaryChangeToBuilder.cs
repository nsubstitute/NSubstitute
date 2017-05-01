namespace NSubstitute.Specs.Infrastructure
{
    public class TemporaryChangeToBuilder<T>
    {
        TemporaryChange<T> _change;

        public TemporaryChangeToBuilder(TemporaryChange<T> change)
        {
            _change = change;
        }

        public void to(T value)
        {
            _change.TemporaryValue = value;
            _change.SetNewValue();
            _change.IsConfigured = true;
        }
    }
}