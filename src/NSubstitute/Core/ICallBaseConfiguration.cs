namespace NSubstitute.Core
{
    public interface ICallBaseConfiguration
    {
        /// <summary>
        /// Gets or sets whether base method should be called by default.
        /// </summary>
        bool CallBaseByDefault { get; set; }

        /// <summary>
        /// Specifies whether base method should be always ignored for the matching call.
        /// If method is both explicitly excluded and included, base method is _not_ called.
        /// </summary>
        void Exclude(ICallSpecification callSpecification);

        /// <summary>
        /// Specifies whether base method should be called for the matching call.
        /// If method is both explicitly excluded and included, base method is _not_ called.
        /// </summary>
        void Include(ICallSpecification callSpecification);

        /// <summary>
        /// Tests whether base method should be called for the call given the existing configuration.
        /// </summary>
        bool ShouldCallBase(ICall call);
    }
}