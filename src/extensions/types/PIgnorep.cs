using System;

namespace Kernel
{
    public class PIgnorep : POperative
    {
        public PIgnorep()
        {
        }

        public override string getName()
        {
            return "ignore?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KIgnore;
        }
    }
}

