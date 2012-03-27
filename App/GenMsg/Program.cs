using System;
using System.Collections.Generic;
using NDesk.Options;

namespace RosSharp.GenMsg
{
    class Program
    {
        static void Main(string[] args)
        {
            string ns = "";
            string outputDir = "";
            string fileName = "";
            var includeDirs = new List<string>();


            var optionSet = new OptionSet()
            {
                {"n|namespace=", v => ns = v},
                {"i|include_dir=", v=>includeDirs.Add(v)},
                {"o|output_dir=", v=>outputDir = v},
                {"f|file_name=", v=>fileName = v}
            };

            optionSet.Parse(args);

            if(string.IsNullOrEmpty(fileName))
            {
                Environment.Exit(0);
            }
            //Generator.generateMessage(fileName, ns, includeDirs);
        }
    }
}
