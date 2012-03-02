using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace RosSharp.Parameter
{
    public class ParameterServerClient
    {
        private ParameterServerProxy _proxy;
        public ParameterServerClient(Uri uri)
        {
            _proxy = new ParameterServerProxy();
            _proxy.Url = uri.ToString();
        }

        public IObservable<int> DeleteParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginDeleteParam, _proxy.EndDeleteParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        public IObservable<int> SetParamAsync(string callerId, string key, object value)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, object, object[]>(_proxy.BeginSetParam, _proxy.EndSetParam)
                .Invoke(callerId, key, value)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        public IObservable<object[]> GetParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginGetParam, _proxy.EndGetParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
        }

        public IObservable<string> SearchParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginSearchParam, _proxy.EndSearchParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (string)ret[2]);
        }

        public IObservable<object[]> SubscribeParamAsync(string callerId, string key, string callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginSubscribeParam, _proxy.EndSubscribeParam)
                .Invoke(callerId, key, callerApi)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
        }

        public IObservable<int> UnsubscribeParamAsync(string callerId, string key, string callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginUnsubscribeParam, _proxy.EndUnsubscribeParam)
                .Invoke(callerId, key, callerApi)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        public IObservable<bool> HasParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginHasParam, _proxy.EndHasParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (bool)ret[2]);
        }

        public IObservable<List<string>> GetParamNamesAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetParamNames, _proxy.EndGetParamNames)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => ((string[])ret[2]).ToList());
        }
    }
}
