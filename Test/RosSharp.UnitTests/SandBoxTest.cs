using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests
{
    [TestClass]
    public class SandBoxTest
    {
        [TestMethod]
        public void Task_Exception()
        {
            //Taskの動き確認

            //RunもFailも表示されない。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task => Console.WriteLine("[1]Run"), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(task => Console.WriteLine("[1]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            //失敗したときにRunだけ呼ばれてしまう。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task => Console.WriteLine("[2]Run"))
                .ContinueWith(task => Console.WriteLine("[2]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            //失敗したときにRunとFailが両方呼ばれてしまう。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task =>
                {
                    Console.WriteLine("[3]Run");
                    return task;
                })
                .Unwrap()
                .ContinueWith(task => Console.WriteLine("[3]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            //RunとFailが両方呼ばれる。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task =>
                {
                    Console.WriteLine("[4]Run");
                    if(task.IsFaulted)
                    {
                        throw task.Exception.InnerException;
                    }
                })
                .ContinueWith(task => Console.WriteLine("[4]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }


        [TestMethod]
        public void Task_ContinueWhenAll()
        {
            {//両方成功ならもちろんSuccess
                var t1 = Task.Factory.StartNew(() => { });
                var t2 = Task.Factory.StartNew(() => { });

                var agg = Task.Factory.ContinueWhenAll(new Task[] {t1, t2}, _ => { });


                agg.ContinueWith(t => Console.WriteLine("Success"), TaskContinuationOptions.OnlyOnRanToCompletion);
                agg.ContinueWith(t => Console.WriteLine("Failure"), TaskContinuationOptions.OnlyOnFaulted);
            }

            {//t1が失敗してもSuccessが表示される！？
                var t1 = Task.Factory.StartNew(() => { throw new Exception("error"); });
                var t2 = Task.Factory.StartNew(() => { });

                var agg = Task.Factory.ContinueWhenAll(new Task[] { t1, t2 }, _ => { });


                agg.ContinueWith(t => Console.WriteLine("Success"), TaskContinuationOptions.OnlyOnRanToCompletion);
                agg.ContinueWith(t => Console.WriteLine("Failure"), TaskContinuationOptions.OnlyOnFaulted);
            }

            {//WaitAllを使うべき？
                var t1 = Task.Factory.StartNew(() => { throw new Exception("error"); });
                var t2 = Task.Factory.StartNew(() => { });

                var agg = Task.Factory.StartNew(() => Task.WaitAll(new Task[] {t1, t2}));

                agg.ContinueWith(t => Console.WriteLine("Success"), TaskContinuationOptions.OnlyOnRanToCompletion);
                agg.ContinueWith(t => Console.WriteLine("Failure"), TaskContinuationOptions.OnlyOnFaulted);
            }
        }


        [TestMethod]
        public void Rx_PublishLastException()
        {
            var subject = new Subject<int>();
            var last = subject
                .Take(1)
                .PublishLast();

            last.Connect();

            subject.OnError(new Exception("hoge"));

            try
            {
                var ret = last.First();
                Console.WriteLine(ret);
            }
            catch (Exception ex)
            {
                //ちゃんと例外通知される。
                Console.WriteLine(ex);
            }
        }

        [TestMethod]
        public void Rx_PublishLastException2()
        {
            var subject = new Subject<int>();
            var last = subject
                .Do(_=>{throw new Exception();})
                .Take(1)
                .PublishLast();

            last.Connect();

            subject.OnNext(1);

            try
            {
                var ret = last.First();
                Console.WriteLine(ret);
            }
            catch (Exception ex)
            {
                //ちゃんと例外通知される。
                Console.WriteLine(ex);
            }
        }

        [TestMethod]
        public void Rx_PublishLastException3()
        {
            var subject = new Subject<int>();
            var last = subject
                .Do(_ => { throw new Exception(); })
                .Take(1)
                .PublishLast();

            last.Connect();

            subject.OnNext(1);

            try
            {
                var ret = last.First();
                Console.WriteLine(ret);
            }
            catch (Exception ex)
            {
                //ちゃんと例外通知される。
                Console.WriteLine(ex);
            }
        }


        [TestMethod]
        public void MultipleSubject()
        {
            var sub1 = new Subject<int>();
            var sub2 = new Subject<int>();
            var sub3 = new Subject<int>();

            var aggregator = new Subject<int>();

            sub1.Subscribe(aggregator);
            sub2.Subscribe(aggregator);

            var dis1 = aggregator.Subscribe(x => Console.WriteLine("Subscriber1={0}", x));
            var dis2 = aggregator.Subscribe(x => Console.WriteLine("Subscriber2={0}", x));

            sub1.OnNext(1);
            sub2.OnNext(10);

            sub3.Subscribe(aggregator);
            var dis3 = aggregator.Subscribe(x => Console.WriteLine("Subscriber3={0}", x));

            sub3.OnNext(100);

            dis1.Dispose();

            sub1.OnNext(2);
            sub2.OnNext(20);
            sub3.OnNext(200);
        }

        [TestMethod]
        public void Rx_Subject()
        {
            var sub = new Subject<int>();

            sub.OnNext(1);

            sub.Subscribe(Console.WriteLine);

            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        [TestMethod]
        public void Subject_test()
        {
            var source1 = new Subject<int>();
            var aggregateSubject = new Subject<int>();
            source1.Subscribe(aggregateSubject);

            aggregateSubject.Subscribe(Console.WriteLine);
            source1.OnNext(123);

            // source1をOnCompletedすると、aggregateのSubscriberも完了する
            source1.OnCompleted();
            

            var source2 = new Subject<int>();
            source2.Subscribe(aggregateSubject);
            source2.OnNext(456);

            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        [TestMethod]
        public void Subject_merge()
        {
            var source1 = new Subject<int>();
            var source2 = new Subject<int>();

            var aggregateSubject = source1.Merge(source2);

            aggregateSubject.Subscribe(Console.WriteLine);
            
            source1.OnNext(123);
            source2.OnNext(456);

            source1.OnCompleted();

            source1.OnNext(999);
            source2.OnNext(789);

            Thread.Sleep(TimeSpan.FromSeconds(3));
        }

        [TestMethod]
        public void Subject_merge2()
        {
            var source1 = new Subject<int>();
            var source2 = new Subject<int>();

            var aggregateSubject = source1.Merge(source2);

            aggregateSubject.Subscribe(Console.WriteLine);

            source1.OnNext(123);
            source2.OnNext(456);

            source1.OnCompleted();

            source1.OnNext(999);
            source2.OnNext(789);

            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}
