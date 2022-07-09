using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        private readonly ICallFactory _callFactory;
        private readonly IArgumentSpecificationDequeue _argSpecificationDequeue;
        private readonly ProxyGenerator _proxyGenerator;
        private readonly AllMethodsExceptCallRouterCallsHook _allMethodsExceptCallRouterCallsHook;

        public CastleDynamicProxyFactory(ICallFactory callFactory, IArgumentSpecificationDequeue argSpecificationDequeue)
        {
            _callFactory = callFactory;
            _argSpecificationDequeue = argSpecificationDequeue;
            _proxyGenerator = new ProxyGenerator();
            _allMethodsExceptCallRouterCallsHook = new AllMethodsExceptCallRouterCallsHook();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, object?[]? constructorArguments)
        {
            return typeToProxy.IsDelegate()
                ? GenerateDelegateProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments)
                : GenerateTypeProxy(callRouter, typeToProxy, additionalInterfaces, constructorArguments);
        }

        private object GenerateTypeProxy(ICallRouter callRouter, Type typeToProxy, Type[]? additionalInterfaces, object?[]? constructorArguments)
        {
            VerifyClassHasNotBeenPassedAsAnAdditionalInterface(additionalInterfaces);

            var proxyIdInterceptor = new ProxyIdInterceptor(typeToProxy);
            var forwardingInterceptor = CreateForwardingInterceptor(callRouter);

            var proxyGenerationOptions = GetOptionsToMixinCallRouterProvider(callRouter);

            var proxy = CreateProxyUsingCastleProxyGenerator(
                typeToProxy,
                additionalInterfaces,
                constructorArguments,
                new IInterceptor[] {proxyIdInterceptor, forwardingInterceptor},
                proxyGenerationOptions);

            forwardingInterceptor.SwitchToFullDispatchMode();
            return proxy;
        }

        private object GenerateDelegateProxy(ICallRouter callRouter, Type delegateType, Type[]? additionalInterfaces, object?[]? constructorArguments)
        {
            VerifyNoAdditionalInterfacesGivenForDelegate(additionalInterfaces);
            VerifyNoConstructorArgumentsGivenForDelegate(constructorArguments);

            var forwardingInterceptor = CreateForwardingInterceptor(callRouter);
            // Keep this interceptor, so that real proxy ID can be retrieved by proxy.Target.ToString().
            var proxyIdInterceptor = new ProxyIdInterceptor(delegateType);

            var proxyGenerationOptions = GetOptionsToMixinCallRouterProvider(callRouter);
            proxyGenerationOptions.AddDelegateTypeMixin(delegateType);

            var proxy = CreateProxyUsingCastleProxyGenerator(
                typeToProxy: typeof(object),
                additionalInterfaces: null,
                constructorArguments: null,
                interceptors: new IInterceptor[] {proxyIdInterceptor, forwardingInterceptor},
                proxyGenerationOptions);

            forwardingInterceptor.SwitchToFullDispatchMode();

            // Ideally we should use ProxyUtil.CreateDelegateToMixin(proxy, delegateType).
            // But it's slower than code below due to extra checks it performs.
            return proxy.GetType().GetInvokeMethod().CreateDelegate(delegateType, proxy);
        }

        private CastleForwardingInterceptor CreateForwardingInterceptor(ICallRouter callRouter)
        {
            return new CastleForwardingInterceptor(
                new CastleInvocationMapper(
                    _callFactory,
                    _argSpecificationDequeue),
                callRouter);
        }

        private object CreateProxyUsingCastleProxyGenerator(Type typeToProxy, Type[]? additionalInterfaces,
                                                            object?[]? constructorArguments,
                                                            IInterceptor[] interceptors,
                                                            ProxyGenerationOptions proxyGenerationOptions)
        {
            if (typeToProxy.GetTypeInfo().IsInterface)
            {
                VerifyNoConstructorArgumentsGivenForInterface(constructorArguments);

                var interfacesArrayLength = additionalInterfaces != null ? additionalInterfaces.Length + 1 : 1;
                var interfaces = new Type[interfacesArrayLength];

                interfaces[0] = typeToProxy;
                if (additionalInterfaces != null)
                {
                    Array.Copy(additionalInterfaces, 0, interfaces, 1, additionalInterfaces.Length);
                }

                // We need to create a proxy for the object type, so we can intercept the ToString() method.
                // Therefore, we put the desired primary interface to the secondary list.
                typeToProxy = typeof(object);
                additionalInterfaces = interfaces;
            }

            return _proxyGenerator.CreateClassProxy(typeToProxy,
                additionalInterfaces,
                proxyGenerationOptions,
                constructorArguments,
                interceptors);
        }

        private ProxyGenerationOptions GetOptionsToMixinCallRouterProvider(ICallRouter callRouter)
        {
            var options = new ProxyGenerationOptions(_allMethodsExceptCallRouterCallsHook);

            // Previously we mixed in the callRouter instance directly, and now we create a wrapper around it.
            // The reason is that we want SubstitutionContext.GetCallRouterFor(substitute) to return us the
            // original callRouter object, rather than the substitute object (as it implemented the ICallRouter interface directly).
            // That need appeared due to the ThreadLocalContext.SetNextRoute() API, which compares the passed callRouter instance by reference.
            options.AddMixinInstance(new StaticCallRouterProvider(callRouter));

            return options;
        }

        private static void VerifyNoConstructorArgumentsGivenForInterface(object?[]? constructorArguments)
        {
            if (HasItems(constructorArguments))
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for an interface.");
            }
        }

        private static void VerifyNoConstructorArgumentsGivenForDelegate(object?[]? constructorArguments)
        {
            if (HasItems(constructorArguments))
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for a delegate.");
            }
        }

        private static void VerifyNoAdditionalInterfacesGivenForDelegate(Type[]? constructorArguments)
        {
            if (HasItems(constructorArguments))
            {
                throw new SubstituteException(
                    "Can not specify additional interfaces when substituting for a delegate. " +
                    "You must specify only a single delegate type if you need to substitute for a delegate.");
            }
        }

        private static void VerifyClassHasNotBeenPassedAsAnAdditionalInterface(Type[]? additionalInterfaces)
        {
            if (additionalInterfaces != null && additionalInterfaces.Any(x => x.GetTypeInfo().IsClass))
            {
                throw new SubstituteException(
                    "Can not substitute for multiple classes. " +
                    "To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.");
            }
        }

        private static bool HasItems<T>(T[]? array) => array?.Length > 0;

        private class AllMethodsExceptCallRouterCallsHook : AllMethodsHook
        {
            public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                // Always intercept object.ToString() as we would like to return proxy id as a result.
                if (ProxyIdInterceptor.IsDefaultToStringMethod(methodInfo))
                    return true;

                return IsNotCallRouterProviderMethod(methodInfo)
                    && IsNotBaseObjectMethod(methodInfo)
                    && base.ShouldInterceptMethod(type, methodInfo);
            }

            private static bool IsNotCallRouterProviderMethod(MethodInfo methodInfo) =>
                methodInfo.DeclaringType != typeof(ICallRouterProvider);

            private static bool IsNotBaseObjectMethod(MethodInfo methodInfo) =>
                methodInfo.GetBaseDefinition().DeclaringType != typeof(object);
        }

        private class StaticCallRouterProvider : ICallRouterProvider
        {
            private readonly ICallRouter _callRouter;

            public StaticCallRouterProvider(ICallRouter callRouter)
            {
                _callRouter = callRouter;
            }

            public ICallRouter GetCallRouter() => _callRouter;
        }
    }
}