#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;

namespace RosSharp.Transport
{
    internal sealed class OneLineCacheSubject<T> : ISubject<T>, IDisposable
    {
        private bool _disposed = false;
        private readonly object _lockObject = new object();
        private readonly List<Notification<T>> _notifications = new List<Notification<T>>();
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

        #region IDisposable Members

        public void Dispose()
        {
            _disposed = true;
        }

        #endregion

        #region ISubject<T> Members

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

        #endregion
    }
}