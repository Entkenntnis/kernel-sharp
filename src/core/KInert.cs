using System;

namespace Kernel
{

    public class KInert : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "#inert";
        }

        public override bool CompareTo(KObject other)
        {
            return other is KInert;
        }
    }
}

