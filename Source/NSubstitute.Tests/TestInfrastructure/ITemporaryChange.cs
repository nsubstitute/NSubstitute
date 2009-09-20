namespace NSubstitute.Tests.TestInfrastructure
{
    public interface ITemporaryChange
    {
        void SetNewValue();
        void RestoreOriginalValue();
    }
}