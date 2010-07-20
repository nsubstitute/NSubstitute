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
        event EventHandler<EventArgs> Broken;
        event EventHandler<LowFuelWarningEventArgs> LowFuelWarning;
    }

    public class IdlingEventArgs : EventArgs
    {
        public int Rpm;
    }

    public class LowFuelWarningEventArgs : EventArgs
    {
        public int PercentLeft { get; private set; }
        public LowFuelWarningEventArgs(int percentLeft)
        {
            PercentLeft = percentLeft;
        }
    }
}