using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public sealed class Name
    {
        private readonly string _name;
        private const char Seperator = '/';

        public Name(string name)
        {
            _name = name;
        }

        public static implicit operator Name(string s)
        {
            return new Name(s);
        }

        public static implicit operator string(Name n)
        {
            return n._name;
        }

    }
}
