using System;

namespace Kernel
{
    public class PBooleanp : POperative
    {
        public PBooleanp()
        {
        }

        public override string getName()
        {
            return "boolean?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KBoolean;
        }
    }
}

