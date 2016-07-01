#if (NET4 || NET45 || NETSTANDARD1_5)
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NSubstitute.Routing.AutoValues
{
    public class AutoTaskProvider : IAutoValueProvider
    {
        private readonly Func<IAutoValueProvider[]> _autoValueProviders;

        public AutoTaskProvider(Func<IAutoValueProvider[]> autoValueProviders)
        {
            _autoValueProviders = autoValueProviders;
        }

        public bool CanProvideValueFor(Type type)
        {
            return typeof (Task).IsAssignableFrom(type);
        }

        public object GetValue(Type type)
        {
            if (!CanProvideValueFor(type))
                throw new InvalidOperationException();

            if (type.IsGenericType())
            {
                var taskType = type.GetGenericArguments()[0];
                var valueProvider = _autoValueProviders().FirstOrDefault(vp => vp.CanProvideValueFor(taskType));

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

        private static object GetDefault(Type type)
        {
            return type.IsValueType() ? Activator.CreateInstance(type) : null;
        }
    }
}
#endif