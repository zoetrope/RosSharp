using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.4:11311/");
            Ros.HostName = "192.168.11.3";
            Ros.XmlRpcTimeout = 3000;
            Ros.TopicTimeout = 3000;

            //SyncMain();
            AsyncMainTAP();
            //AsyncMain();

            Thread.Sleep(Timeout.Infinite);
            Ros.Dispose();
        }


        static void SyncMain()
        {
            try
            {
                var node = Ros.InitNodeAsync("/Listener").Result;
                var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
                subscriber.WaitForConnection();
                subscriber.Subscribe(x => Console.WriteLine(x.data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static void AsyncMainTAP()
        {
            Ros.InitNodeAsync("/Listener")
                .ContinueWith(node =>
                {
                    return node.Result.SubscriberAsync<RosSharp.std_msgs.String>("/chatter");
                })
                .Unwrap()
                .ContinueWith(subscriber =>
                {
                    subscriber.Result.Subscribe(x => Console.WriteLine(x.data));
                })
                .ContinueWith(res =>
                {
                    Console.WriteLine("失敗！！！！！！！！！！！");
                    //Console.WriteLine(res.Exception.InnerException);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /*
        static async void AsyncMain()
        {
            try
            {
                var node = await Ros.InitNodeAsync("/Listener");
                var subscriber = await node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter");
                subscriber.Subscribe(x => Console.WriteLine(x.data));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }
}
