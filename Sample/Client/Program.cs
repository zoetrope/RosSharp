using System;
using System.Threading;
using System.Threading.Tasks;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.2:11311/");
            Ros.HostName = "192.168.11.2";
            Ros.XmlRpcTimeout = 3000;
            Ros.TopicTimeout = 3000;

            //SyncMain();
            AsyncMainTAP();
            //AsyncMain();

            Console.WriteLine("Press Any Key.");
            Console.ReadKey();
        }

        static void SyncMain()
        {
            try
            {
                var node = Ros.InitNodeAsync("/Client").Result;
                var proxy = node.ServiceProxyAsync<AddTwoInts>("/add_two_ints").Result;

                var res1 = proxy.Invoke(new AddTwoInts.Request() { a = 1, b = 2 });
                Console.WriteLine(res1.sum);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        static void AsyncMainTAP()
        {
            Ros.InitNodeAsync("/Client")
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
                    Console.WriteLine(res.Result.sum);
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
                var node = await Ros.InitNodeAsync("/Client");
                var proxy = await node.ServiceProxyAsync<AddTwoInts>("/add_two_ints");

                var res1 = await proxy.InvokeAsync(new AddTwoInts.Request() { a = 1, b = 2 });
                Console.WriteLine(res1.sum);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        */
    }
}
