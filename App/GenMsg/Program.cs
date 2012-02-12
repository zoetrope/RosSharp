using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace GenMsg
{
    class Program
    {
        static void Main(string[] args)
        {
            string data;
            var p = new OptionSet() {
                { "f|file=", v => data = v }
            };

            List<string> extra = p.Parse(args);


        }
    }
}
