using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    ///         - <see cref="NSubLifetime.Transient" />
    ///         - <see cref="NSubLifetime.PerScope" />
    ///         - <see cref="NSubLifetime.Singleton" />
    ///     - Immutability (via interfaces) and customization by creating a nested container
    /// </summary>
    public class NSubContainer : IConfigurableNSubContainer
    {
        private readonly NSubContainer? _parentContainer;
        private readonly object _syncRoot;
        private readonly Dictionary<Type, Registration> _registrations = new();

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

        public T Resolve<T>() where T : notnull => CreateScope().Resolve<T>();

        public IConfigurableNSubContainer Register<TKey, TImpl>(NSubLifetime lifetime)
            where TKey : notnull
            where TImpl : TKey
        {
            var constructors = typeof(TImpl).GetConstructors();
            if (constructors.Length != 1)
            {
                throw new ArgumentException(
                    $"Type '{typeof(TImpl).FullName}' should contain only single public constructor. " +
                    $"Please register type using factory method to avoid ambiguity.");
            }

            var ctor = constructors[0];

            object Factory(Scope scope)
            {
                var args = ctor.GetParameters()
                    .Select(p => scope.Resolve(p.ParameterType))
                    .ToArray();
                return ctor.Invoke(args);
            }

            SetRegistration(typeof(TKey), new Registration(Factory, lifetime));

            return this;
        }

        public IConfigurableNSubContainer Register<TKey>(Func<INSubResolver, TKey> factory, NSubLifetime lifetime)
            where TKey: notnull
        {
            object Factory(Scope scope)
            {
                return factory.Invoke(scope);
            }

            SetRegistration(typeof(TKey), new Registration(Factory, lifetime));

            return this;
        }

        public IConfigurableNSubContainer Decorate<TKey>(Func<TKey, INSubResolver, TKey> factory)
            where TKey : notnull
        {
            Registration? existingRegistration = TryFindRegistration(typeof(TKey));
            if (existingRegistration == null)
            {
                throw new ArgumentException("Cannot decorate type " + typeof(TKey).FullName +" as implementation is not registered.");
            }

            object Factory(Scope scope)
            {
                // Freeze original registration discovered during decoration.
                // This way we avoid recursion and support nested decorators.
                var originalInstance = (TKey) scope.Resolve(existingRegistration);
                return factory.Invoke(originalInstance, scope);
            }

            SetRegistration(typeof(TKey), new Registration(Factory, existingRegistration.Lifetime));

            return this;
        }

        public IConfigurableNSubContainer Customize()
        {
            return new NSubContainer(this);
        }

        public INSubResolver CreateScope()
        {
            return new Scope(this);
        }

        private void SetRegistration(Type type, Registration registration)
        {
            lock (_syncRoot)
            {
                _registrations[type] = registration;
            }
        }

        private Registration? TryFindRegistration(Type type)
        {
            // Both read and write accesses to dictionary should be synchronized.
            // The same lock object is shared among all the nested containers,
            // so we synchronize across the whole containers graph.
            lock (_syncRoot)
            {
                var currentContainer = this;
                while (currentContainer != null)
                {
                    if (currentContainer._registrations.TryGetValue(type, out var registration))
                    {
                        return registration;
                    }

                    currentContainer = currentContainer._parentContainer;
                }

                return null;
            }
        }

        private class Registration
        {
            private readonly Func<Scope, object> _factory;
            private object? _singletonValue;
            public NSubLifetime Lifetime { get; }

            public Registration(Func<Scope, object> factory, NSubLifetime lifetime)
            {
                _factory = factory;
                Lifetime = lifetime;
            }

            public object Resolve(Scope scope)
            {
                switch (Lifetime)
                {
                    case NSubLifetime.Transient:
                        return _factory.Invoke(scope);

                    case NSubLifetime.Singleton:
                        return _singletonValue ??= _factory.Invoke(scope);

                    case NSubLifetime.PerScope:
                        if (scope.TryGetCached(this, out var result))
                        {
                            return result;
                        }

                        result = _factory.Invoke(scope);
                        scope.Set(this, result);
                        return result;

                    default:
                        throw new InvalidOperationException("Unknown lifetime");
                }
            }
        }

        private class Scope : INSubResolver
        {
            private readonly Dictionary<Registration, object> _cache = new Dictionary<Registration, object>();
            private readonly NSubContainer _mostNestedContainer;

            public Scope(NSubContainer mostNestedContainer)
            {
                _mostNestedContainer = mostNestedContainer;
            }

            public T Resolve<T>() where T : notnull => (T) Resolve(typeof(T));

            public bool TryGetCached(Registration registration, [MaybeNullWhen(false)] out object result)
            {
                return _cache.TryGetValue(registration, out result);
            }

            public void Set(Registration registration, object value)
            {
                _cache[registration] = value;
            }

            public object Resolve(Type type)
            {
                // The same lock object is shared among all the nested containers,
                // so we synchronize across the whole containers graph.
                lock (_mostNestedContainer._syncRoot)
                {
                    Registration? registration = _mostNestedContainer.TryFindRegistration(type);
                    if (registration == null)
                        throw new InvalidOperationException($"Type is not registered: {type.FullName}");

                    return registration.Resolve(this);
                }
            }

            public object Resolve(Registration registration)
            {
                // The same lock object is shared among all the nested containers,
                // so we synchronize across the whole containers graph.
                lock (_mostNestedContainer._syncRoot)
                {
                    return registration.Resolve(this);
                }
            }
        }
    }
}
