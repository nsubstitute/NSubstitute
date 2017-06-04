namespace NSubstitute.Specs.Infrastructure
{
    public interface ITemporaryChange
    {
        void SetNewValue();
        void RestoreOriginalValue();
        string MemberName { get; }
        bool IsConfigured { get; set; }
    }
}