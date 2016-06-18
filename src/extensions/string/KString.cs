using System;

namespace Kernel
{
    public class KString : KObject
    {
        public string Value
        {
            get;
            private set;
        }
        public KString(string val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            if (quoteStrings)
                return ToLiteral(Value);
            else
                return Value;
        }
        private static string ToLiteral(string input)
        {
            return "\"" + input.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("\t", "\\t").Replace("\r", "\\r") + "\"";
        }
        public override bool CompareTo(KObject other)
        {
            return other is KString && (other as KString).Equals(Value);
        }
    }
}

