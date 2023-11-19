using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSubstitute.Specs
{
#if NET45
    public class AnonymousObserver<T> : IObserver<T>
    {
        Action<T> _onNext;
        Action<Exception> _onError;
        Action _onCompleted;

        public AnonymousObserver(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
        {
            _onNext = onNext ?? (_ => { });
            _onError = onError ?? (_ => { });
            _onCompleted = onCompleted ?? (() => {});
        }

        public void OnNext(T value) { _onNext(value); }
        public void OnError(Exception error) { _onError(error); }
        public void OnCompleted() { _onCompleted(); }
    }
#endif
}
