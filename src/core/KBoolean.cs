using System;

namespace Kernel
{

    public class KBoolean : KObject
    {
        public bool Value {
            get;
            private set;
        }

        public KBoolean(bool value)
        {
            Value = value;
        }

        public override string Print(bool quoteStrings)
        {
            return Value ? "#t" : "#f";
        }
    }
}

