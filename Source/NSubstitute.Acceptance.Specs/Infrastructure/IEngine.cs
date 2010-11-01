using System;

namespace NSubstitute.Acceptance.Specs.Infrastructure
{
    public interface IEngine
    {
        void Start();
        void Rev();
        void Stop();
        void Idle();
        void RevAt(int rpm);
        void FillPetrolTankTo(int percent);
        float GetCapacityInLitres();
        event Action Started;
    }
}