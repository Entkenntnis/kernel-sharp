using System;

namespace Kernel
{
    public class PVau : POperative
    {
        public override string getName()
        {
            return "$vau";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 3);
            KObject formalt = First(args), eformal = Second(args), expr = Third(args);
            return new KOperative(formalt, eformal, expr, env);
        }
    }
}

