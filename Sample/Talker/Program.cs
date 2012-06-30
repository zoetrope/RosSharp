using System;
using System.Threading;
using System.Threading.Tasks;
using RosSharp;

namespace Sample
{
    /// <summary>
    /// Sample code for Publisher
    /// </summary>
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


        /// <summary>
        /// Synchronous version
        /// </summary>
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

        /// <summary>
        /// Asynchronous version by TAP (Task-based Asynchronous Pattern)
        /// </summary>
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
                    Console.WriteLine(res.Exception.InnerException.Message);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /*
        /// <summary>
        /// Asynchronous version by using async/await
        /// </summary>
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
