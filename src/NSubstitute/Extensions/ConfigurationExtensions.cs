using NSubstitute.Core;

namespace NSubstitute.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// A hint for the NSubstitute that the subsequent method/property call is about to be configured.
        /// <para>
        /// NOTICE, you _don't need_ to invoke this method for the basic configuration scenarios.
        /// Ensure you don't overuse this method and it's applied only if strictly required.
        /// </para>
        /// <remarks>
        /// Due to the NSubstitute configuration syntax it's often impossible to recognize during the method call
        /// dispatch whether this is a setup phase or a regular method call.
        /// Usually, it doesn't matter, however sometimes method invocation could lead to undesired side effects
        /// (e.g. the previously configured value is returned, base method is invoked). In that case you might want to
        /// provide NSubstitute with a hint that you are configuring the method, so it treats call in a special mode.
        /// </remarks>
        /// </summary>
        public static T Configure<T>(this T substitute) where T : class
        {
            var context = SubstitutionContext.Current;
            var router = context.GetCallRouterFor(substitute);
            var routeFactory = context.GetRouteFactory();

            router.SetRoute(state => routeFactory.RecordCallSpecification(state));

            return substitute;
        }
    }
}