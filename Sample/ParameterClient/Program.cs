using System;
using System.Threading.Tasks;
using RosSharp;

namespace Sample
{
    /// <summary>
    /// Sample code for Parameter client
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
                var node = Ros.InitNodeAsync("/ParameterSample").Result;
                var param = node.PrimitiveParameterAsync<string>("/test_param").Result;
                
                param.Subscribe(x => Console.WriteLine(x));
                param.Value = "test";
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
            Ros.InitNodeAsync("/ParameterSample")
                .ContinueWith(node =>
                {
                    return node.Result.PrimitiveParameterAsync<string>("/test_param");
                })
                .Unwrap()
                .ContinueWith(param =>
                {
                    param.Result.Subscribe(x => Console.WriteLine(x));
                    param.Result.Value = "test";
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
                var node = await Ros.InitNodeAsync("/ParameterSample");
                var param = await node.PrimitiveParameterAsync<string>("/test_param");
                
                param.Subscribe(x => Console.WriteLine(x));
                param.Value = "test";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }
}
