using System;

namespace Kernel
{
    public class PCombinerp : POperative
    {
        public PCombinerp()
        {
        }

        public override string getName()
        {
            return "combiner?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KCombiner;
        }
    }
}

