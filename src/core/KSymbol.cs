using System;

namespace Kernel
{

    public class KSymbol : KObject
    {
        public string Value {
            get;
            private set;
        }

        public KSymbol(string value)
        {
            Value = value;
        }

        public override string Print(bool quoteStrings)
        {
            return Value;
        }

        public override bool CompareTo(KObject other)
        {
            return other is KSymbol && (other as KSymbol).Value == Value;
        }
    }
}

