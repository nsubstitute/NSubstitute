using System;

namespace NSubstitute.Core.DependencyInjection
{
    public interface INSubResolver
    {
        T Resolve<T>() where T : notnull;
    }

    public interface INSubContainer : INSubResolver
    {
        /// <summary>
        /// Creates a new container based on the current one,
        /// which can be configured to override the existing registrations without affecting the existing container.
        /// </summary>
        IConfigurableNSubContainer Customize();

        /// <summary>
        /// Create an explicit scope, so all dependencies with the <see cref="NSubLifetime.PerScope"/> lifetime
        /// are preserved for multiple resolve requests.
        /// </summary>
        INSubResolver CreateScope();
    }

    public interface IConfigurableNSubContainer : INSubContainer
    {
        IConfigurableNSubContainer Register<TKey, TImpl>(NSubLifetime lifetime)
            where TKey : notnull
            where TImpl : TKey;

        IConfigurableNSubContainer Register<TKey>(Func<INSubResolver, TKey> factory, NSubLifetime lifetime)
            where TKey : notnull;

        /// <summary>
        /// Decorates the original implementation with a custom decorator.
        /// The factory method is provided with an original implementation instance.
        /// The lifetime of decorated implementation is used.
        /// </summary>
        IConfigurableNSubContainer Decorate<TKey>(Func<TKey, INSubResolver, TKey> factory)
            where TKey : notnull;
    }

    public static class ConfigurableNSubContainerExtensions
    {
        public static IConfigurableNSubContainer RegisterPerScope<TKey, TImpl>(this IConfigurableNSubContainer container)
            where TKey : notnull
            where TImpl : TKey
        {
            return container.Register<TKey, TImpl>(NSubLifetime.PerScope);
        }

        public static IConfigurableNSubContainer RegisterPerScope<TKey>(this IConfigurableNSubContainer container, Func<INSubResolver, TKey> factory)
            where TKey : notnull
        {
            return container.Register(factory, NSubLifetime.PerScope);
        }

        public static IConfigurableNSubContainer RegisterSingleton<TKey, TImpl>(this IConfigurableNSubContainer container)
            where TKey : notnull
            where TImpl : TKey
        {
            return container.Register<TKey, TImpl>(NSubLifetime.Singleton);
        }
    }
}