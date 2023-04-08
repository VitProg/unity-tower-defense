using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace td.common
{
    public class MessageBus
    {
        private readonly ISubject<object> _messageStream = new Subject<object>();

        public void Publish<T>(T message)
        {
            _messageStream.OnNext(message);
        }

        public IObservable<T> GetStream<T>()
        {
            return _messageStream.OfType<T>();
        }

        public IObservable<T> GetStream<T>(Func<T, bool> predicate)
        {
            return _messageStream.OfType<T>().Where(predicate);
        }
    }
}