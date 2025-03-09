using System.Diagnostics;

namespace NSubstitute.Core;

[DebuggerDisplay("{" + nameof(_name) + "}")]
public sealed class MatchArgs
{
    private readonly string _name;

    public static readonly MatchArgs AsSpecifiedInCall = new(nameof(AsSpecifiedInCall));
    public static readonly MatchArgs Any = new(nameof(Any));

    private MatchArgs(string name)
    {
        _name = name;
    }
}