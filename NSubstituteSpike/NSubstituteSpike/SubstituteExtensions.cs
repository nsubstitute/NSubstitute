namespace NSubstituteSpike
{
    public static class SubstituteExtensions
    {
        public static void Return<T>(this T valueBeingExtended, T returnThis)
        {
            Substitute.LastCallShouldReturn(returnThis);
        }
    }
}