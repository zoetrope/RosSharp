using System;
using System.Threading.Tasks;
using RosSharp.Master;

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

        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("add_two_ints: a = {0}, b = {1}", req.a, req.b);
            return new AddTwoInts.Response() { sum = req.a + req.b };
        }

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
                    Console.WriteLine("失敗！！！！！！！！！！！");
                    //Console.WriteLine(res.Exception.InnerException);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /*
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
