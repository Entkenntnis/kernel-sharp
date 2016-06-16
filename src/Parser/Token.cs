using System;

namespace Kernel
{
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
}

