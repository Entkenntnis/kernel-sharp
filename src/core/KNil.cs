using System;

namespace Kernel
{

    public class KNil : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "()";
        }

        public override bool CompareTo(KObject other)
        {
            return other is KNil;
        }
    }
}

