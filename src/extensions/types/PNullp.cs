using System;

namespace Kernel
{
    public class PNullp : POperative
    {
        public PNullp()
        {
        }

        public override string getName()
        {
            return "null?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KNil;
        }
    }
}

