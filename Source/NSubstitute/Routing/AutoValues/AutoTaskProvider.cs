#if (NET4 || NET45)
using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoTaskProvider : IAutoValueProvider, IMaybeAutoValueProvider
    {
        private readonly Func<IAutoValueProvider[]> _autoValueProviders;
        private readonly Func<IMaybeAutoValueProvider[]> _maybeAutoValueProviders;

        public AutoTaskProvider(Func<IAutoValueProvider[]> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }
        public AutoTaskProvider(Func<IAutoValueProvider[]> autoValueProviders, Func<IMaybeAutoValueProvider[]> maybeAutoValueProviders)
            : this(autoValueProviders)
        {
            _maybeAutoValueProviders = maybeAutoValueProviders;
        }

        public bool CanProvideValueFor(Type type)
        {
            return typeof (Task).IsAssignableFrom(type);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            if (type.IsGenericType)
            {
                var taskType = type.GetGenericArguments()[0];
                var valueProvider = _autoValueProviders().FirstOrDefault(vp => vp.CanProvideValueFor(taskType));
                
                var value = valueProvider == null ? GetDefault(taskType) : valueProvider.GetValue(taskType);
                var taskCompletionSourceType = typeof(TaskCompletionSource<>).MakeGenericType(taskType);
                var taskCompletionSource = Activator.CreateInstance(taskCompletionSourceType);
                taskCompletionSourceType.GetMethod("SetResult").Invoke(taskCompletionSource, new[] { value });
                return taskCompletionSourceType.GetProperty("Task").GetValue(taskCompletionSource, null);
            }
            else
            {
                var taskCompletionSource = new TaskCompletionSource<object>();
                taskCompletionSource.SetResult(null);
                return taskCompletionSource.Task;
            }
        }

        private object GetDefault(Type type)
        {
            if (_maybeAutoValueProviders == null)
                return null;

            return _maybeAutoValueProviders()
                .Select(vp => vp.GetValue(type))
                .FirstOrDefault(vp => vp.HasValue())
                .ValueOrDefault();
        }

        Maybe<object> IMaybeAutoValueProvider.GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetValue(type));
        }
    }
}
#endif