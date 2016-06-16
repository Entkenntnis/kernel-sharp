using System;

namespace Kernel
{
    public class TextHandler
    {
        public int Precedance { get; private set; }
        public Func<Token, object> Handler { get; private set; }
        public TextHandler(int prec, Func<Token, object> h)
        {
            Precedance = prec;
            Handler = h;
        }
    }
}

