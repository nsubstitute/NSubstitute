using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        private readonly IArgumentSpecificationDequeue _argSpecificationDequeue;
        private readonly ProxyGenerator _proxyGenerator;
        private readonly AllMethodsExceptCallRouterCallsHook _allMethodsExceptCallRouterCallsHook;

        public CastleDynamicProxyFactory(IArgumentSpecificationDequeue argSpecificationDequeue)
        {
            _argSpecificationDequeue = argSpecificationDequeue;
            _proxyGenerator = new ProxyGenerator();
            _allMethodsExceptCallRouterCallsHook = new AllMethodsExceptCallRouterCallsHook();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            VerifyClassHasNotBeenPassedAsAnAdditionalInterface(additionalInterfaces);

            var proxyIdInterceptor = new ProxyIdInterceptor(typeToProxy);
            var forwardingInterceptor = new CastleForwardingInterceptor(
                new CastleInvocationMapper(
                    new CallFactory(),
                    _argSpecificationDequeue),
                callRouter);

            var proxyGenerationOptions = GetOptionsToMixinCallRouter(callRouter);
            var proxy = CreateProxyUsingCastleProxyGenerator(
                typeToProxy,
                additionalInterfaces,
                constructorArguments,
                new IInterceptor[] {proxyIdInterceptor, forwardingInterceptor},
                proxyGenerationOptions);

            forwardingInterceptor.StartIntercepting();
            return proxy;
        }

        /// <summary>
        /// Dynamically define the type with specified <paramref name="typeName"/>.
        /// </summary>
        public TypeBuilder DefineDynamicType(string typeName, TypeAttributes flags)
        {
            // It's important to use the signed module, as we exposed internals type to the signed module.
            return _proxyGenerator.ProxyBuilder.ModuleScope.DefineType(inSignedModulePreferably: true, typeName, flags);
        }

        private object CreateProxyUsingCastleProxyGenerator(Type typeToProxy, Type[] additionalInterfaces,
                                                            object[] constructorArguments,
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

        private ProxyGenerationOptions GetOptionsToMixinCallRouter(ICallRouter callRouter)
        {
            var options = new ProxyGenerationOptions(_allMethodsExceptCallRouterCallsHook);
            options.AddMixinInstance(callRouter);
            return options;
        }

        private static void VerifyNoConstructorArgumentsGivenForInterface(object[] constructorArguments)
        {
            if (constructorArguments != null && constructorArguments.Length > 0)
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for an interface.");
            }
        }

        private static void VerifyClassHasNotBeenPassedAsAnAdditionalInterface(Type[] additionalInterfaces)
        {
            if (additionalInterfaces != null && additionalInterfaces.Any(x => x.GetTypeInfo().IsClass))
            {
                throw new SubstituteException("Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.");
            }
        }

        private class AllMethodsExceptCallRouterCallsHook : AllMethodsHook
        {
            public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                // Always intercept object.ToString() as we would like to return proxy id as a result.
                if (ProxyIdInterceptor.IsDefaultToStringMethod(methodInfo))
                    return true;

                return IsNotCallRouterMethod(methodInfo)
                    && IsNotBaseObjectMethod(methodInfo)
                    && base.ShouldInterceptMethod(type, methodInfo);
            }

            private static bool IsNotCallRouterMethod(MethodInfo methodInfo)
            {
                return methodInfo.DeclaringType != typeof(ICallRouter);
            }

            private static bool IsNotBaseObjectMethod(MethodInfo methodInfo)
            {
                return methodInfo.GetBaseDefinition().DeclaringType != typeof(object);
            }
        }
    }
}