using System;

namespace Kernel
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message){ }
    }
}

