using System;
using System.Numerics;
using System.Globalization;

namespace Kernel
{
    public class KInteger : KObject
    {
        public BigInteger Value {
            get;
            private set;
        }
        public KInteger(BigInteger value)
        {
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            return Value.ToString();
        }
        public override bool CompareTo(KObject other)
        {
            return other is KInteger && (other as KInteger).Value.Equals(Value);
        }
    }
}

