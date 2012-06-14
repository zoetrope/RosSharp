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

            Ros.Dispose();
        }


        static void SyncMain()
        {
            try
            {
                var node = Ros.CreateNodeAsync("/Talker").Result;

                var publisher = node.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;

                publisher.WaitForConnection();

                int i = 0;
                while (true)
                {
                    var data = new RosSharp.std_msgs.String() { data = "test : " + i++ };
                    Console.WriteLine("data = {0}", data.data);
                    publisher.OnNext(data);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static void AsyncMainTAP()
        {
            Ros.CreateNodeAsync("/Talker")
                .ContinueWith(node =>
                {
                    return node.Result.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter");
                })
                .Unwrap()
                .ContinueWith(publisher =>
                {
                    int i = 0;
                    while (true)
                    {
                        var data = new RosSharp.std_msgs.String() { data = "test : " + i++ };
                        Console.WriteLine("data = {0}", data.data);
                        publisher.Result.OnNext(data);
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }

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
                var node = await Ros.CreateNodeAsync("/Talker");

                var publisher = await node.CreatePublisherAsync<RosSharp.std_msgs.String>("/chatter");

                int i = 0;
                while (true)
                {
                    var data = new RosSharp.std_msgs.String() { data = "test : " + i++ };
                    Console.WriteLine("data = {0}", data.data);
                    publisher.OnNext(data);
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }
}
