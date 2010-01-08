namespace NSubstitute.Acceptance.Specs
{
    public interface IEngine
    {
        void Start();
        void Rev();
        void Stop();
        void Idle();
        void RevAt(int rpm);
    }
}