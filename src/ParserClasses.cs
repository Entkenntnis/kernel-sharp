using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Kernel
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message){ }
    }

    public class TokenDefinition
    {
        public Regex Rule { get; private set; }
        public string Label { get; private set; }
        public bool Ignore { get; private set; }
        public int Precedence { get; private set; }
        public TokenDefinition(Regex rule, string label, int prec = 0, bool ignore = false)
        {
            Rule = rule;
            Label = label;
            Ignore = ignore;
            Precedence = prec;
        }
    }

    public class Token
    {
        public string Value { get; private set; }
        public string Label { get; private set; }
        public Token(string value, string label)
        {
            Value = value;
            Label = label;
        }
        public override string ToString()
        {
            return string.Format("[Token: Value={0}, Label={1}]", Value, Label);
        }
    }

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

