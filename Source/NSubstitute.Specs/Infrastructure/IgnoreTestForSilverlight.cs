using System;

namespace NSubstitute.Specs.Infrastructure
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
