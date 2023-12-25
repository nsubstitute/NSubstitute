using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class PendingAttribute : CategoryAttribute
{
    public PendingAttribute() : base("Pending") { }
}