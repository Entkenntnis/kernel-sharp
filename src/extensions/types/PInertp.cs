using System;

namespace Kernel
{
    public class PInertp : POperative
    {
        public PInertp()
        {
        }

        public override string getName()
        {
            return "inert?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KInert;
        }
    }
}

