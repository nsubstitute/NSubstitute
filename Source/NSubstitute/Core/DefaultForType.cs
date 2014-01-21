using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NSubstitute.Core
{
    public class DefaultForType : IDefaultForType
    {
        public object GetDefaultFor(Type type)
        {
            if (IsVoid(type)) return null;
            if (type.IsValueType) return DefaultInstanceOfValueType(type);

            if (type == typeof(Task)) 
            {
                // NB: http://stackoverflow.com/questions/11969208/non-generic-taskcompletionsource-or-alternative
                TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }

            if (typeof(Task).IsAssignableFrom(type) && type.IsGenericType) 
            {
                Type innerType = type.GetGenericArguments()[0];
                Type tcsType = typeof(TaskCompletionSource<>).MakeGenericType(innerType);
                object tcs = Activator.CreateInstance(tcsType);

                object result = innerType.IsValueType ? 
                    DefaultInstanceOfValueType(innerType) :
                    null;

                MethodInfo setResult = tcsType.GetMethod("SetResult");
                setResult.Invoke(tcs, new[] { result });

                PropertyInfo task = tcsType.GetProperty("Task");
                return task.GetGetMethod().Invoke(tcs, null);
            }

            return null;
        }

        private bool IsVoid(Type returnType)
        {
            return returnType == typeof(void);
        }

        private object DefaultInstanceOfValueType(Type returnType)
        {
            return Activator.CreateInstance(returnType);
        }
    }
}