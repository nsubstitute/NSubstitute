using System.Diagnostics;

namespace NSubstitute.Core
{
    [DebuggerDisplay("{" + nameof(GetDebugDisplayName) + "}")]
    public class MatchArgs
    {
        public static readonly MatchArgs AsSpecifiedInCall = new MatchArgs();
        public static readonly MatchArgs Any = new MatchArgs();

        private MatchArgs() { }

        private string GetDebugDisplayName()
        {
            if (this == AsSpecifiedInCall) return nameof(AsSpecifiedInCall);
            if (this == Any) return nameof(Any);

            return "UNKNOWN";
        }
    }
}