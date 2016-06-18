using System;

namespace Kernel
{
    public class PWrap : POperative
    {
        public PWrap()
        {
        }

        public override string getName()
        {
            return "wrap";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KCombiner op = First(args) as KCombiner;
            Check(op, "not a combiner");
            return new KApplicative(op);
        }
    }
}

