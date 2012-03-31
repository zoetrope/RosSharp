using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

            //以下のようにすると、RunもFailも表示されない。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task => Console.WriteLine("[1]Run"), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(task => Console.WriteLine("[1]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            //以下のようにすると、失敗したときにRunだけ呼ばれてしまう。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task => Console.WriteLine("[2]Run"))
                .ContinueWith(task => Console.WriteLine("[2]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            //以下のようにすると、失敗したときにRunとFailが両方呼ばれてしまう。
            Task.Factory.StartNew(() => { throw new Exception(); })
                .ContinueWith(task =>
                {
                    Console.WriteLine("[3]Run");
                    return task;
                })
                .Unwrap()
                .ContinueWith(task => Console.WriteLine("[3]Fail"), TaskContinuationOptions.OnlyOnFaulted);

            Thread.Sleep(TimeSpan.FromSeconds(5));
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
    }
}
