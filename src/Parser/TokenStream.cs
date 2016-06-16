using System;
using System.Collections.Generic;

namespace Kernel
{
    public class TokenStream
    {
        private int index;
        private List<Token> data;
        public TokenStream(List<Token> input)
        {
            data = new List<Token>(input);
            index = 0;
        }
        public Token Next()
        {
            if (index < data.Count)
            {
                return data[index++];
            }
            else
            {
                return null;
            }
        }
    }
}

