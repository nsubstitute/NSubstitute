namespace NSubstitute.Core
{
    public static class CallSequence
    {
        private static int _sequence = 1;

        public static int GetNextId()
        {
            return _sequence++;
        }
    }
}