using System;

namespace Kernel
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message){ }
    }
}

