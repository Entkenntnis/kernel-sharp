using System;

namespace Kernel
{
    public class PContinuation2Applicative : POperative
    {
        public override string getName()
        {
            return "continuation->applicative";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KContinuation c = First(args) as KContinuation;
            Check(c, "expected continuation");
            return new KApplicative(new PPassCont(c));
        }
    }
}

