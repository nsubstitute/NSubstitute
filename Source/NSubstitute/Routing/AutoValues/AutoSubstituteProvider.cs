using System;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoSubstituteProvider : IAutoValueProvider
    {
        private static readonly string[] _whitelistedClassFullNames = new []
        {
            "System.Web.HttpApplicationStateBase",
            "System.Web.HttpBrowserCapabilitiesBase",
            "System.Web.HttpCachePolicyBase",
            "System.Web.HttpContextBase",
            "System.Web.HttpFileCollectionBase",
            "System.Web.HttpPostedFileBase",
            "System.Web.HttpRequestBase",
            "System.Web.HttpResponseBase",
            "System.Web.HttpServerUtilityBase",
            "System.Web.HttpSessionStateBase",
            "System.Web.HttpStaticObjectsCollectionBase"
        };

        private readonly ISubstituteFactory _substituteFactory;

        public AutoSubstituteProvider(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public bool CanProvideValueFor(Type type)
        {
            return type.IsInterface
                || type.IsSubclassOf(typeof(Delegate))
                || _whitelistedClassFullNames.Contains(type.FullName);
        }

        public object GetValue(Type type)
        {
            return _substituteFactory.Create(new[] { type }, new object[0]);
        }
    }
}