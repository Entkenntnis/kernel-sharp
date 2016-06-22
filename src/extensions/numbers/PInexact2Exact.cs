using System;
using System.Numerics;

namespace Kernel
{
    public class PInexact2Exact : POperative
    {
        public PInexact2Exact()
        {
        }

        public override string getName()
        {
            return "inexact->exact";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            var inex = First(args) as KDouble;
            Check(inex);
            return KFraction.fromDouble(inex.Value);
        }
    }
}

