using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Text;

namespace RosSharp.Transport
{

    internal sealed class OneLineCacheSubject<T> : ISubject<T>, IDisposable
    {
        private object _lockObject = new object();
        private List<IObserver<T>> _observers = new List<IObserver<T>>();
        private List<Notification<T>> _notifications = new List<Notification<T>>();
        private bool _disposed = false;

        public void OnNext(T value)
        {
            if (_disposed) throw new ObjectDisposedException("OneLineCacheSubject");

            var next = Notification.CreateOnNext(value);

            lock (_lockObject)
            {
                if (_observers.Count == 0)
                {
                    _notifications.Add(next);
                }
            }
            _observers.ForEach(next.Accept);
        }

        public void OnError(Exception error)
        {
            if (_disposed) throw new ObjectDisposedException("OneLineCacheSubject");

            var err = Notification.CreateOnError<T>(error);

            lock (_lockObject)
            {
                if (_observers.Count == 0)
                {
                    _notifications.Add(err);
                }
            }
            _observers.ForEach(err.Accept);
            
        }

        public void OnCompleted()
        {
            if (_disposed) throw new ObjectDisposedException("OneLineCacheSubject");

            var completed = Notification.CreateOnCompleted<T>();

            lock (_lockObject)
            {
                if (_observers.Count == 0)
                {
                    _notifications.Add(completed);
                }
            }
            _observers.ForEach(completed.Accept);
            
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            _observers.Add(observer);
            lock (_lockObject)
            {
                if (_notifications.Any())
                {
                    _notifications.ForEach(n => n.Accept(observer));
                    _notifications.Clear();
                }
            }
            return Disposable.Create(() => _observers.Remove(observer));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
