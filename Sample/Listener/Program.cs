using System;
using System.Threading.Tasks;
using RosSharp;

namespace Sample
{
    /// <summary>
    /// Sample code for Subscriber
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
                var node = Ros.InitNodeAsync("/Listener").Result;
                var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
                subscriber.Subscribe(x => Console.WriteLine(x.data));
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
