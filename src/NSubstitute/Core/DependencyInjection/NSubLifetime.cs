namespace NSubstitute.Core.DependencyInjection
{
    public enum NSubLifetime
    {
        /// <summary>
        /// Value is created only once.
        /// </summary>
        Singleton,
        /// <summary>
        /// Value is created only once per scope. Allows to share the same instance across the objects in the same graph.
        /// If no explicit scope is created, an implicit scope is created per single resolve request.
        /// </summary>
        PerScope,
        /// <summary>
        /// New value is created for each time.
        /// </summary>
        Transient
    }
}