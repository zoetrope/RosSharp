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
            //SyncMain();
            AsyncMainTAP();
            //AsyncMain();

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
            Ros.Dispose();
        }


        static void SyncMain()
        {
            try
            {
                var node = Ros.InitNodeAsync("/Talker").Result;
                var publisher = node.PublisherAsync<RosSharp.std_msgs.String>("/chatter").Result;

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
            Ros.InitNodeAsync("/Talker")
                .ContinueWith(node =>
                {
                    return node.Result.PublisherAsync<RosSharp.std_msgs.String>("/chatter");
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
                    Console.WriteLine(res.Exception.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /*
        static async void AsyncMain()
        {
            try
            {
                var node = await Ros.InitNodeAsync("/Talker");
                var publisher = await node.PublisherAsync<RosSharp.std_msgs.String>("/chatter");

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
