using System;

namespace Kernel
{
    public class PPairp : POperative
    {
        public PPairp()
        {
        }

        public override string getName()
        {
            return "pair?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KPair;
        }
    }
}

