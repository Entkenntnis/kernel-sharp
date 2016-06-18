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
            double input = inex.Value;
            bool negativ = false;
            if (input < 0) {
                negativ = true;
                input *= -1;
            }
            BigInteger num = (BigInteger)Math.Floor(input);
            BigInteger denom = 1;
            double rem = input - Math.Floor(input);
            while (rem > 0) {
                num *= 2;
                denom *= 2;
                rem *= 2.0;
                num += (BigInteger)Math.Floor(rem);
                rem -= Math.Floor(rem);
            }
            if (negativ)
                num *= -1;
            return new KFraction(num, denom);
        }
    }
}

