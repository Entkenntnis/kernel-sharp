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

        public override bool CompareTo(KObject other)
        {
            return other is KBoolean && (other as KBoolean).Value == Value;
        }
    }
}

