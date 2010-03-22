namespace NSubstitute.Specs.Infrastructure
{
    public interface ITemporaryChange
    {
        void SetNewValue();
        void RestoreOriginalValue();
    }
}