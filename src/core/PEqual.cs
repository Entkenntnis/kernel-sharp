using System;
using System.Collections.Generic;

namespace Kernel
{
    public class PEqual : POperative
    {
        public override string getName()
        {
            return "equal?";
        }
        public static bool CompareEqual(KObject a, KObject b)
        {
            if (a.CompareTo(b))
                return true;
            else
            {
                if (a is KPair && b is KPair)
                {
                    KPair ap = a as KPair;
                    KPair bp = b as KPair;
                    return (CompareEqual(ap.Car, bp.Car) && CompareEqual(ap.Cdr, bp.Cdr));
                }
            }
            return false;
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 2);
            KObject a = First(args);
            KObject b = Second(args);
            return CompareEqual(a, b);
        }
    }
}

