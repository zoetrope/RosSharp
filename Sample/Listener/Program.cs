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
                var node = Ros.CreateNodeAsync("/Listener").Result;
                var subscriber = node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
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
            Ros.CreateNodeAsync("/Listener")
                .ContinueWith(node =>
                {
                    return node.Result.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter");
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
                var node = await Ros.CreateNodeAsync("/Listener");
                var subscriber = await node.CreateSubscriberAsync<RosSharp.std_msgs.String>("/chatter");
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
