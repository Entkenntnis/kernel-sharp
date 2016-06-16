using System;

namespace Kernel
{

    public class KIgnore : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "#ignore";
        }

        public override bool CompareTo(KObject other)
        {
            return other is KIgnore;
        }
    }
}

