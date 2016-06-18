using System;

namespace Kernel
{
    public class PExact2Inexact : POperative
    {
        public PExact2Inexact()
        {
        }

        public override string getName()
        {
            return "exact->inexact";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KFraction input = First(args) as KFraction;
            Check(input);
            double num = (double)input.Numerator;
            double denom = (double)input.Denominator;
            return new KDouble(num/denom);
        }
    }
}

