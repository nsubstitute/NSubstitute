using NSubstitute.Core.Arguments;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Routing;
using NSubstitute.Routing.AutoValues;

namespace NSubstitute.Core.DependencyInjection
{
    public static class NSubstituteDefaultFactory
    {
        /// <summary>
        /// The default NSubstitute registrations. Feel free to configure the existing container to customize
        /// and override NSubstitute parts.
        /// </summary>
        public static INSubContainer DefaultContainer { get; } = CreateDefaultContainer();

        public static ISubstitutionContext CreateSubstitutionContext() => DefaultContainer.Resolve<ISubstitutionContext>();

        private static INSubContainer CreateDefaultContainer()
        {
            return new NSubContainer()
                .RegisterSingleton<SequenceNumberGenerator, SequenceNumberGenerator>()
                .RegisterPerScope<IThreadLocalContext, ThreadLocalContext>()
                .RegisterPerScope<IArgumentSpecificationDequeue>(r =>
                    new ArgumentSpecificationDequeue(r.Resolve<IThreadLocalContext>().DequeueAllArgumentSpecifications))
                .RegisterPerScope<ICallSpecificationFactory, CallSpecificationFactory>()
                .RegisterPerScope<IArgumentSpecificationFactory, ArgumentSpecificationFactory>()
                .RegisterPerScope<IArgumentSpecificationsFactory, ArgumentSpecificationsFactory>()
                .RegisterPerScope<ISuppliedArgumentSpecificationsFactory, SuppliedArgumentSpecificationsFactory>()
                .RegisterPerScope<IArgumentSpecificationCompatibilityTester, ArgumentSpecificationCompatibilityTester>()
                .RegisterPerScope<IDefaultChecker, DefaultChecker>()
                .RegisterPerScope<IDefaultForType, DefaultForType>()
                .RegisterPerScope<IRouteFactory, RouteFactory>()
                .RegisterPerScope<ICallInfoFactory, CallInfoFactory>()
                .RegisterPerScope<IAutoValueProvidersFactory, AutoValueProvidersFactory>()
                .RegisterPerScope<ISubstituteStateFactory, SubstituteStateFactory>()
                .RegisterPerScope<ICallRouterFactory, CallRouterFactory>()
                .RegisterPerScope<ISubstituteFactory, SubstituteFactory>()
                .RegisterPerScope<ICallRouterResolver, CallRouterResolver>()
                .RegisterPerScope<ISubstitutionContext, SubstitutionContext>()
                .RegisterPerScope<IProxyFactory, CastleDynamicProxyFactory>()
                .RegisterPerScope<ICallFactory, CallFactory>()
                .RegisterPerScope<IPropertyHelper, PropertyHelper>()
                .RegisterSingleton<IReceivedCallsExceptionThrower, ReceivedCallsExceptionThrower>();
        }
    }
}