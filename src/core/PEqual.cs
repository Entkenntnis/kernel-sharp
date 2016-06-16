using System;
using System.Collections.Generic;

namespace Kernel
{
    public class PEqual : KOperative
    {
        public static bool CompareEqual(KObject a, KObject b, List<KObject> visited)
        {
            if (a.CompareTo(b))
                return true;
            if (visited.Contains(a) && visited.Contains(b))
                return true;
            else
            {
                if (a is KPair && b is KPair)
                {
                    if (!visited.Contains(a))
                        visited.Add(a);
                    if (!visited.Contains(b))
                        visited.Add(b);
                    KPair ap = a as KPair;
                    KPair bp = b as KPair;
                    return (CompareEqual(ap.Car, bp.Car, visited) && CompareEqual(ap.Cdr, bp.Cdr, visited));
                }
            }
            return false;
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "equal?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args);
            KObject b = Second(args);
            return ReturnBool(CompareEqual(a, b, new List<KObject>()), cont);
        }
    }
}

