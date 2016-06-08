#if (NET4 || NET45)
using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Core;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoTaskProvider : IAutoValueProvider
    {
        private readonly Func<IAutoValueProvider[]> _autoValueProviders;

        public AutoTaskProvider(Func<IAutoValueProvider[]> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        private bool CanProvideValueFor(Type type)
        {
            return typeof (Task).IsAssignableFrom(type);
        }

        private object GetActualValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            if (type.IsGenericType)
            {
                var taskType = type.GetGenericArguments()[0];
                var value = GetValueFromProvider(taskType);
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

        private object GetValueFromProvider(Type type)
        {
            if (_autoValueProviders == null)
                return null;

            return _autoValueProviders()
                .Select(vp => vp.GetValue(type))
                .FirstOrDefault(vp => vp.HasValue())
                .ValueOrDefault();
        }

        public Maybe<object> GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                return Maybe.Nothing<object>();

            return Maybe.Just(GetActualValue(type));
        }
    }
}
#endif