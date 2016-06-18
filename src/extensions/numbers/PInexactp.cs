using System;

namespace Kernel
{
    public class PInexactp :POperative
    {
        public PInexactp()
        {
        }

        public override string getName()
        {
            return "inexact?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KDouble;
        }
    }
}

