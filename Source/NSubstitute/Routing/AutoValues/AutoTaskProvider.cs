#if NET4
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoTaskProvider : IAutoValueProvider
    {
        private readonly IAutoValueProvider[] _autoValueProviders;

        public AutoTaskProvider(IAutoValueProvider[] autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        public bool CanProvideValueFor(Type type)
        {
            if (typeof(Task).IsAssignableFrom(type))
            {
                if (type.IsGenericType)
                {
                    var taskType = type.GetGenericArguments()[0];
                    return _autoValueProviders.Any(vp => vp.CanProvideValueFor(taskType));
                }

                return true;
            }

            return false;
        }

        public object GetValue(Type type)
        {
            if (!typeof(Task).IsAssignableFrom(type))
                throw new InvalidOperationException();

            if (type.IsGenericType)
            {
                var taskType = type.GetGenericArguments()[0];
                var valueProvider = _autoValueProviders.FirstOrDefault(vp => vp.CanProvideValueFor(taskType));

                var value = valueProvider == null ? GetDefault(type) : valueProvider.GetValue(taskType);
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

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
#endif