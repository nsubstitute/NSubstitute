namespace NSubstitute.Specs.TestInfrastructure
{
    public class TemporaryChangeToBuilder<T>
    {
        TemporaryChange<T> _change;

        public TemporaryChangeToBuilder(TemporaryChange<T> change)
        {
            _change = change;
        }

        public TemporaryChangeViaBuilder<T> to(T value)
        {
            _change.TemporaryValue = value;
            return new TemporaryChangeViaBuilder<T>(_change);
        }
    }
}