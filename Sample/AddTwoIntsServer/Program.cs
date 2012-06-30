using System;
using System.Threading.Tasks;
using RosSharp;

namespace Sample
{
    /// <summary>
    /// Sample code for Service server
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

        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("add_two_ints: a = {0}, b = {1}", req.a, req.b);
            return new AddTwoInts.Response() { sum = req.a + req.b };
        }

        /// <summary>
        /// Synchronous version
        /// </summary>
        static void SyncMain()
        {
            try
            {
                var node = Ros.InitNodeAsync("/Server").Result;
                var server = node.AdvertiseServiceAsync("/add_two_ints", new AddTwoInts(add_two_ints)).Result;
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
            Ros.InitNodeAsync("/Server")
                .ContinueWith(node =>
                {
                    return node.Result.AdvertiseServiceAsync("/add_two_ints", new AddTwoInts(add_two_ints));
                })
                .Unwrap()
                .ContinueWith(server =>
                {
                    server.Wait();
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
                var node = await Ros.InitNodeAsync("/Server");
                var server = await node.AdvertiseServiceAsync("/add_two_ints", new AddTwoInts(add_two_ints));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }


}
