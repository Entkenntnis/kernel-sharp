using System;
using System.Globalization;

namespace Kernel
{
    public class KDouble : KObject
    {
        public double Value {
            get;
            private set;
        }
        public KDouble(double value)
        {
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            if (Double.IsPositiveInfinity(Value))
                return "#e+infinity";
            if (Double.IsNegativeInfinity(Value))
                return "#e-infinity";
            string val = Value.ToString(CultureInfo.InvariantCulture);
            if (!val.Contains("."))
                val += ".0";
            return val;
        }
        public override bool CompareTo(KObject other)
        {
            return other is KDouble && (other as KDouble).Value.Equals(Value);
        }
    }
}

