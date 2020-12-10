using System;
using NSubstitute.Core;
using NSubstitute.Exceptions;

// Disable nullability for client API, so it does not affect clients.
#nullable disable annotations

namespace NSubstitute.Extensions
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// A hint for the NSubstitute that the subsequent method/property call is about to be configured.
        /// For example: substitute.Configure().GetValue().Returns(1,2,3);
        /// <para>
        /// NOTICE, you _don't need_ to invoke this method for the basic configuration scenarios.
        /// Ensure you don't overuse this method and it is applied only if strictly required.
        /// </para>
        /// <remarks>
        /// Due to the NSubstitute configuration syntax it is often impossible to recognise during the method call
        /// dispatch whether this is a setup phase or a regular method call.
        /// Usually it doesn't matter, however sometimes method invocation could lead to undesired side effects
        /// (e.g. the previously configured value is returned, base method is invoked). In that case you might want to
        /// provide NSubstitute with a hint that you are configuring a method, so it handles the call in configuration mode.
        /// </remarks>
        /// </summary>
        public static T Configure<T>(this T substitute) where T : class
        {
            if (substitute == null) throw new NullSubstituteReferenceException();
 
            var context = SubstitutionContext.Current;
            var callRouter = context.GetCallRouterFor(substitute!);
            context.ThreadContext.SetNextRoute(callRouter, context.RouteFactory.RecordCallSpecification);

            return substitute;
        }
    }
}