namespace NSubstitute.Core
{
    public enum SubstituteConfig
    {
        /// <summary>
        /// Standard substitute behaviour; replace all calls with substituted behaviour.
        /// </summary>
        OverrideAllCalls,
        /// <summary>
        /// Partial substitute; use base behaviour unless explicitly overriden.
        /// </summary>
        CallBaseByDefault
    }
}