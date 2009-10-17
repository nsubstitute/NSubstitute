namespace NSubstitute.Specs.TestInfrastructure
{
    public interface ITemporaryChange
    {
        void SetNewValue();
        void RestoreOriginalValue();
    }
}