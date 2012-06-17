using System;
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
                var node = Ros.InitNodeAsync("/Listener").Result;
                var subscriber = node.SubscriberAsync<RosSharp.std_msgs.String>("/chatter").Result;
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
                    Console.WriteLine(res.Exception.Message);
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
