namespace NSubstituteSpike
{
    public interface ISubstitute
    {
        Substitute Instance { get; set; }
        void LastCallShouldReturn(object o);
    }
}