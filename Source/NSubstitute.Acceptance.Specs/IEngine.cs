using System;

namespace NSubstitute.Acceptance.Specs
{
    public interface IEngine
    {
        void Start();
        void Rev();
        void Stop();
        void Idle();
        void RevAt(int rpm);
        event Action Started;
        event EventHandler<IdlingEventArgs> Idling;

    }

    public class IdlingEventArgs : EventArgs
    {
        public int Rpm;
    }
}