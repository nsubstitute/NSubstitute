using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NSubstitute.Core.DependencyInjection
{
    /// <summary>
    /// Tiny and very limited implementation of the DI services.
    /// Container supports the following features required by NSubstitute:
    ///     - Registration by type with automatic constructor injection
    ///     - Registration of factory methods for the complex objects
    ///     - Support of the most required lifetimes:
    ///         - <see cref="NSubLifetime.Transient"/>
    ///         - <see cref="NSubLifetime.PerScope"/>
    ///         - <see cref="NSubLifetime.Singleton"/>
    ///     - Immutability (via interfaces) and customization by creating a nested container
    /// </summary>
    public class NSubContainer : IConfigurableNSubContainer
    {
        private readonly NSubContainer _parentContainer;
        private readonly object _syncRoot;

        private Dictionary<Type, Registration> Registrations { get; } = new Dictionary<Type, Registration>();

        public NSubContainer()
        {
            _syncRoot = new object();
        }

        private NSubContainer(NSubContainer parentContainer)
        {
            _parentContainer = parentContainer;

            // Use the same synchronization object in descendant containers.
            // It's required to e.g. ensure that singleton dependencies are resolved only once.
            _syncRoot = parentContainer._syncRoot;
        }

        public T Resolve<T>()
        {
            return (T) ResolveImpl(typeof(T), new ScopeCache());
        }

        public IConfigurableNSubContainer Register<TKey, TImpl>(NSubLifetime lifetime) where TImpl : TKey
        {
            var constructors = typeof(TImpl).GetConstructors();
            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    $"Type '{typeof(TImpl).FullName}' should contain only single public constructor. " +
                    $"Please register type using factory method to avoid ambiguity.");
            }

            var ctor = constructors[0];

            object Factory(ScopeCache scopeCache)
            {
                var args = ctor.GetParameters()
                    .Select(p => ResolveImpl(p.ParameterType, scopeCache))
                    .ToArray();
                return ctor.Invoke(args);
            }

            AddRegistration(typeof(TKey), new Registration(Factory, lifetime));

            return this;
        }

        public IConfigurableNSubContainer Register<TKey>(Func<INSubResolver, TKey> factory, NSubLifetime lifetime)
        {
            object Factory(ScopeCache scopeCache)
            {
                return factory.Invoke(new ScopeCacheBoundResolver(this, scopeCache));
            }
            
            AddRegistration(typeof(TKey), new Registration(Factory, lifetime));

            return this;
        }

        public IConfigurableNSubContainer Customize()
        {
            return new NSubContainer(this);
        }

        public INSubResolver CreateScope()
        {
            return new ScopeCacheBoundResolver(this, new ScopeCache());
        }

        private void AddRegistration(Type type, Registration registration)
        {
            lock (_syncRoot)
            {
                Registrations[type] = registration;
            }
        }
        
        private object ResolveImpl(Type type, ScopeCache scopeCache)
        {
            lock (_syncRoot)
            {
                if (Registrations.TryGetValue(type, out var registration))
                {
                    return registration.Resolve(scopeCache);
                }

                if (_parentContainer != null)
                {
                    return _parentContainer.ResolveImpl(type, scopeCache);
                }

                throw new InvalidOperationException($"Type is not registered: {type.FullName}");
            }
        }

        private class Registration
        {
            private readonly NSubLifetime _lifetime;
            private readonly Func<ScopeCache, object> _factory;
            private object _singletonValue;

            public Registration(Func<ScopeCache, object> factory, NSubLifetime lifetime)
            {
                _factory = factory;
                _lifetime = lifetime;
            }

            public object Resolve(ScopeCache scopeCache)
            {
                switch (_lifetime)
                {
                    case NSubLifetime.Transient:
                        return _factory.Invoke(scopeCache);

                    case NSubLifetime.Singleton:
                        return _singletonValue ?? (_singletonValue = _factory.Invoke(scopeCache));

                    case NSubLifetime.PerScope:
                        if (scopeCache.TryGetValue(this, out var result))
                        {
                            return result;
                        }

                        result = _factory.Invoke(scopeCache);
                        scopeCache.Set(this, result);
                        return result;

                    default:
                        throw new InvalidOperationException("Unknown lifetime.");
                }
            }
        }

        private class ScopeCache
        {
            private readonly Dictionary<Registration, object> _cache = new Dictionary<Registration, object>();

            public bool TryGetValue(Registration registration, out object result) =>
                _cache.TryGetValue(registration, out result);

            public void Set(Registration registration, object value) =>
                _cache[registration] = value;
        }

        private class ScopeCacheBoundResolver : INSubResolver
        {
            private readonly NSubContainer _container;
            private readonly ScopeCache _scopeCache;

            public ScopeCacheBoundResolver(NSubContainer container, ScopeCache scopeCache)
            {
                _container = container;
                _scopeCache = scopeCache;
            }

            public T Resolve<T>() =>
                (T) _container.ResolveImpl(typeof(T), _scopeCache);
        }
    }
}