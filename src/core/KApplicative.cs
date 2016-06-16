
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
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Combiner.Combine(args, env, cont);
        }

        public override bool CompareTo(KObject other)
        {
            return other is KApplicative && other == this;
        }
    }
}

