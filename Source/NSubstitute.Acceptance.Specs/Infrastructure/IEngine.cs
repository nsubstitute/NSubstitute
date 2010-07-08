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
        void FillPetrolTankTo(int percent);
        event Action Started;
        event Action<int> RevvedAt;
        event Action<int, int> PetrolTankFilled;
        event EventHandler<IdlingEventArgs> Idling;
        event EventHandler Stopped;

    }

    public class IdlingEventArgs : EventArgs
    {
        public int Rpm;
    }
}