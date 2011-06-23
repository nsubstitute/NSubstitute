using System;

namespace NSubstitute.Acceptance.Specs
{
#if SILVERLIGHT
    public class IgnoreTestForSilverlightAttribute : NUnit.Framework.IgnoreAttribute
    {
        public IgnoreTestForSilverlightAttribute () : base("Excluded for Silverlight test runs") { }
    }
#else
    public class IgnoreTestForSilverlightAttribute : Attribute { }
#endif
}