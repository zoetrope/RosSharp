using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RosSharp
{

    internal delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    internal delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    internal delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    internal static class ObservableEx
    {
        // http://neue.cc/2010/09/28_277.html
        
        public static Func<T1, T2,T3, IObservable<TResult>> FromAsyncPattern<T1, T2,T3, TResult>(Func<T1, T2, T3, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return (t1, t2, t3) =>
            {
                var subject = new AsyncSubject<TResult>();
                begin.Invoke(t1, t2, t3, iar =>
                {
                    TResult value;
                    try
                    {
                        value = end.Invoke(iar);
                    }
                    catch (Exception error)
                    {
                        subject.OnError(error);
                        return;
                    }
                    subject.OnNext(value);
                    subject.OnCompleted();
                }
                , null);
                return subject.AsObservable();
            };
        }
        public static Func<T1, T2, T3, T4, IObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return (t1, t2, t3, t4) =>
            {
                var subject = new AsyncSubject<TResult>();
                begin.Invoke(t1, t2, t3, t4, iar =>
                {
                    TResult value;
                    try
                    {
                        value = end.Invoke(iar);
                    }
                    catch (Exception error)
                    {
                        subject.OnError(error);
                        return;
                    }
                    subject.OnNext(value);
                    subject.OnCompleted();
                }
                , null);
                return subject.AsObservable();
            };
        }
        public static Func<T1, T2, T3, T4, T5, IObservable<TResult>> FromAsyncPattern<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end)
        {
            return (t1, t2, t3, t4, t5) =>
            {
                var subject = new AsyncSubject<TResult>();
                begin.Invoke(t1, t2, t3, t4, t5, iar =>
                {
                    TResult value;
                    try
                    {
                        value = end.Invoke(iar);
                    }
                    catch (Exception error)
                    {
                        subject.OnError(error);
                        return;
                    }
                    subject.OnNext(value);
                    subject.OnCompleted();
                }
                , null);

                return subject.AsObservable();
            };
        }
    }
}
