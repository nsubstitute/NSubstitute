namespace NSubstitute.Core
{
    /// <summary>
    /// Information for a call that returns a value of type <c>T</c>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CallInfo<T> : CallInfo
    {
        internal CallInfo(CallInfo info) : base(info) {
        }

        public T BaseResult() {
            return (T)GetBaseResult();
        }
    }
}