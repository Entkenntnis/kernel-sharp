using System;

namespace Kernel
{
    public class PExactp : POperative
    {
        public PExactp()
        {
        }

        public override string getName()
        {
            return "exact?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KFraction;
        }
    }
}

