using System;
using System.Threading.Tasks;
using RosSharp;

namespace Sample
{
    /// <summary>
    /// Sample code for Service client
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Ros.HostName = "192.168.11.2";
            Ros.MasterUri = new Uri("http://192.168.11.2:11311/");
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
                var node = Ros.InitNodeAsync("/Client").Result;
                node.WaitForService("add_two_ints").Wait(TimeSpan.FromSeconds(10));
                var proxy = node.ServiceProxyAsync<AddTwoInts>("add_two_ints").Result;

                var res = proxy.Invoke(new AddTwoInts.Request() { a = 1, b = 2 });
                Console.WriteLine("Sum = {0}", res.sum);
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
            Ros.InitNodeAsync("/Client")
                .ContinueWith(node =>
                {
                    return node.Result.WaitForService("/add_two_ints").ContinueWith(_ => node.Result);
                })
                .Unwrap()
                .ContinueWith(node =>
                {
                    return node.Result.ServiceProxyAsync<AddTwoInts>("/add_two_ints");
                })
                .Unwrap()
                .ContinueWith(proxy =>
                {
                    return proxy.Result.InvokeAsync(new AddTwoInts.Request() { a = 1, b = 2 });
                })
                .Unwrap()
                .ContinueWith(res=>
                {
                    Console.WriteLine("Sum = {0}", res.Result.sum);
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
                var node = await Ros.InitNodeAsync("/Client");
                await node.WaitForService("/add_two_ints");
                var proxy = await node.ServiceProxyAsync<AddTwoInts>("/add_two_ints");

                var res = await proxy.InvokeAsync(new AddTwoInts.Request() { a = 1, b = 2 });
                Console.WriteLine("Sum = {0}", res.sum);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }
}
