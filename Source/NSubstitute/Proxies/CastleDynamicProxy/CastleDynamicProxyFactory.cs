using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using NSubstitute.Core;
using NSubstitute.Exceptions;

namespace NSubstitute.Proxies.CastleDynamicProxy
{
    public class CastleDynamicProxyFactory : IProxyFactory
    {
        readonly ProxyGenerator _proxyGenerator;
        readonly CastleInterceptorFactory _interceptorFactory;
        readonly AllMethodsExceptCallRouterCallsHook _allMethodsExceptCallRouterCallsHook;

        public CastleDynamicProxyFactory(CastleInterceptorFactory interceptorFactory)
        {
#pragma warning disable 618
                        AttributesToAvoidReplicating.Add<SecurityPermissionAttribute>();
#pragma warning restore 618

#if !SILVERLIGHT
                        AttributesToAvoidReplicating.Add<ReflectionPermissionAttribute>();
                        AttributesToAvoidReplicating.Add<PermissionSetAttribute>();
                        AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.MarshalAsAttribute>();
  #if NET4
                        AttributesToAvoidReplicating.Add<System.Runtime.InteropServices.TypeIdentifierAttribute>();
  #endif
#endif
         
            _proxyGenerator = new ProxyGenerator();
            _interceptorFactory = interceptorFactory;
            _allMethodsExceptCallRouterCallsHook = new AllMethodsExceptCallRouterCallsHook();
        }

        public object GenerateProxy(ICallRouter callRouter, Type typeToProxy, Type[] additionalInterfaces, object[] constructorArguments)
        {
            VerifyClassHasNotBeenPassedAsAnAdditionalInterface(additionalInterfaces);

            var interceptor = _interceptorFactory.CreateForwardingInterceptor(callRouter);
            var proxyGenerationOptions = GetOptionsToMixinCallRouter(callRouter);
            if (typeToProxy.IsInterface)
            {
                VerifyNoConstructorArgumentsGivenForInterface(constructorArguments);
                return _proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, additionalInterfaces, proxyGenerationOptions, interceptor);
            }
            return _proxyGenerator.CreateClassProxy(typeToProxy, additionalInterfaces, proxyGenerationOptions, constructorArguments, interceptor);

        }

        private ProxyGenerationOptions GetOptionsToMixinCallRouter(ICallRouter callRouter)
        {
            var options = new ProxyGenerationOptions(_allMethodsExceptCallRouterCallsHook);
            options.AddMixinInstance(callRouter);
            return options;
        }

        private class AllMethodsExceptCallRouterCallsHook : AllMethodsHook
        {
            public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
            {
                return IsNotCallRouterMethod(methodInfo) && base.ShouldInterceptMethod(type, methodInfo);
            }

            private static bool IsNotCallRouterMethod(MethodInfo methodInfo)
            {
                return methodInfo.DeclaringType != typeof(ICallRouter);
            }
        }

        private void VerifyNoConstructorArgumentsGivenForInterface(object[] constructorArguments)
        {
            if (constructorArguments != null && constructorArguments.Length > 0)
            {
                throw new SubstituteException("Can not provide constructor arguments when substituting for an interface.");
            }
        }

        private void VerifyClassHasNotBeenPassedAsAnAdditionalInterface(Type[] additionalInterfaces)
        {
            if (additionalInterfaces != null && additionalInterfaces.Any(x => x.IsClass))
            {
                throw new SubstituteException("Can not substitute for multiple classes. To substitute for multiple types only one type can be a concrete class; other types can only be interfaces.");
            }
        }

    }
}