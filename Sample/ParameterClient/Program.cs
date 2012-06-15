using System;
using System.Threading.Tasks;

namespace RosSharp.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Ros.MasterUri = new Uri("http://192.168.11.5:11311/");
            Ros.HostName = "192.168.11.3";
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
                    Console.WriteLine("失敗！！！！！！！！！！！");
                    //Console.WriteLine(res.Exception.InnerException);
                }, TaskContinuationOptions.OnlyOnFaulted);
        }
        /*
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
