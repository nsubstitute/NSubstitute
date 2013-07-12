namespace NSubstitute
{
    /// <summary>
    /// Represents the substitute behavior regarding base implementation
    /// </summary>
    public enum ThatCallsBase
    {
        /// <summary>
        /// The substitute calls base implementation only when it's specified explicitly.
        /// </summary>
        OnlyWhenSpecified = 0,
        
        /// <summary>
        /// The substitute calls base implementation by default. Make sure that you substitute for a class.
        /// </summary>
        ByDefault,
    }
}
