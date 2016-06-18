using System;

namespace Kernel
{
    public class PCons : POperative
    {
        public override string getName()
        {
            return "cons";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 2);
            return new KPair(First(args), Second(args));
        }
    }
}

