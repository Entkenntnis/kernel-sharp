using System;

namespace Kernel
{
    public class PEnvironmentp : POperative
    {
        public PEnvironmentp()
        {
        }

        public override string getName()
        {
            return "environment?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KEnvironment;
        }
    }
}

