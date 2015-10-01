using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;

namespace NSubstitute
{
    public static class Callbacks
    {
        public static ICallback First(Action<CallInfo> doThis)
        {
            return new Callback().First(doThis);
        }

        public static ICallback Always(Action<CallInfo> doThis)
        {
            return new Callback().Always(doThis);
        }

        public static ICallback FirstThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new Callback().FirstThrow(throwThis);
        }

        public static ICallback FirstThrow<TException>(TException exception) where TException : Exception
        {
            return new Callback().FirstThrow(info => exception);
        }

        public static ICallback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return new Callback().AlwaysThrow(throwThis);
        }

        public static ICallback AlwaysThrow<TException>(TException exception) where TException : Exception
        {
            return new Callback().AlwaysThrow(info => exception);
        }
    }

    public class Callback : ICallback
    {
        private readonly List<Action<CallInfo>> callbackQueue = new List<Action<CallInfo>>();
        private Action<CallInfo> alwaysDo;
        private Stack<Action<CallInfo>> callbackStack;

        public ICallback FirstThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            return AddThrowCallback(throwThis);
        }

        public ICallback FirstThrow<TException>(TException exception) where TException : Exception
        {
            return ThenThrow(ci => exception);
        }

        public ICallback ThenThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            callbackQueue.Add(ci => { if (throwThis != null) throw throwThis(ci); });
            return this;
        }

        public ICallback ThenThrow<TException>(TException exception) where TException : Exception
        {
            return ThenThrow(ci => exception);
        }

        public ICallback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            alwaysDo = ci => { if (throwThis != null) throw throwThis(ci); };
            return this;
        }

        public ICallback AlwaysThrow<TException>(TException exception) where TException : Exception
        {
            return AlwaysThrow(ci => exception);
        }

        public ICallback Always(Action<CallInfo> doThis)
        {
            alwaysDo = doThis;
            return this;
        }

        public ICallback Then(Action<CallInfo> doThis)
        {
            callbackQueue.Add(doThis);
            return this;
        }

        public ICallback First(Action<CallInfo> doThis)
        {
            return AddCallback(doThis);
        }

        private ICallback AddThrowCallback<TException>(Func<CallInfo, TException> throwThis) where TException : Exception
        {
            callbackQueue.Add(ci => { if (throwThis != null) throw throwThis(ci); });
            return this;
        }

        private ICallback AddCallback(Action<CallInfo> doThis)
        {
            callbackQueue.Add(doThis);
            return this;
        }

        public void Call(CallInfo callInfo)
        {
            InitCallbackStack();

            SafeCall(callInfo);
        }

        private void SafeCall(CallInfo callInfo)
        {
            Exception ex = null;
            try
            {
                CallFromStack(callInfo);
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                CallAlways(callInfo);
            }

            if (ex != null)
                throw ex;
        }

        private void CallAlways(CallInfo callInfo)
        {
            if (alwaysDo != null)
                alwaysDo(callInfo);
        }

        private void CallFromStack(CallInfo callInfo)
        {
            if (callbackStack.Any())
                callbackStack.Pop()(callInfo);
        }

        private void InitCallbackStack()
        {
            if (callbackStack == null)
            {
                callbackQueue.Reverse();
                callbackStack = new Stack<Action<CallInfo>>(callbackQueue);
            }
        }
    }

    public interface ICallback
    {
        ICallback ThenThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception;

        ICallback ThenThrow<TException>(TException exception) where TException : Exception;

        ICallback AlwaysThrow<TException>(Func<CallInfo, TException> throwThis) where TException : Exception;

        ICallback AlwaysThrow<TException>(TException exception) where TException : Exception;

        ICallback Always(Action<CallInfo> doThis);

        ICallback Then(Action<CallInfo> doThis);
    }
}