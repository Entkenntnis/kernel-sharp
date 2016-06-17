
using System;

namespace Kernel
{
    public class KApplicative : KCombiner
    {
        public KCombiner Combiner
        {
            get;
            private set;
        }
        public KApplicative(KCombiner op)
        {
            Combiner = op;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<applicative:" + Combiner.Print(quoteStrings) + ">";
        }

        public override bool CompareTo(KObject other)
        {
            return other is KApplicative && other == this;
        }
    }
}

