using System;
using System.Collections.Generic;
using NDesk.Options;

namespace RosSharp.GenMsg
{    
    class Program
    {
        static void Main(string[] args)
        {
            string generateType = "";
            string ns = "";
            string outputDir = @".\";
            var includeDirs = new List<string>();

            var optionSet = new OptionSet()
            {
                {"t|type=", v => generateType = v},
                {"n|namespace=", v => ns = v},
                {"i|include_dir=", v => includeDirs.Add(v)},
                {"o|output_dir=", v => outputDir = v}
            };

            var files = optionSet.Parse(args);

            if (files.Count == 0 || (generateType != "msg" && generateType != "srv"))
            {
                Console.WriteLine("Usage: GenMsg -t msg|srv [-n namespace] [-o output_dir] [[-i include_dir]...] FileName");
                Environment.Exit(0);
            }

            if (generateType == "msg")
            {
                files.ForEach(file => Generator.generateMessage(file, ns, outputDir, includeDirs));
            }
            else if(generateType == "srv")
            {
                files.ForEach(file => Generator.generateService(file, ns, outputDir, includeDirs));
            }

            Console.WriteLine("Success.");

            Console.ReadKey();
        }
    }
}
