using System;
using System.Collections.Generic;

namespace Kernel
{
    public class PEqual : KOperative
    {
        public static bool compareEq(KObject a, KObject b)
        {
            if (a.GetType().Equals(b.GetType()))
            {
                if (a is KIgnore)
                    return true;
                else if (a is KInert)
                    return true;
                else if (a is KNil)
                    return true;
                else if (a is KIgnore)
                    return true;
                if (a is KBoolean)
                    return ((KBoolean)a).Value == ((KBoolean)b).Value;
                else if (a is KSymbol)
                    return ((KSymbol)a).Value.Equals(((KSymbol)b).Value);
                /*else if (a is KInteger)
                    return ((KInteger)a).Value == ((KInteger)b).Value;
                else if (a is KDouble)
                    return((KDouble)a).Value == ((KDouble)b).Value;
                else if (a is KString)
                    return ((KString)a).Value.Equals(((KString)b).Value);*/
                else if (a is KPair)
                    return a == b;
                else if (a is KEnvironment)
                    return a == b;
                else if (a is KOperative)
                    return a == b;
                else if (a is KApplicative)
                    return a == b;
                /*else if (a is KEncapsulation)
                    return a == b;
                else if (a is KContinuation)
                    return a == b;*/
            }
            return false;
        }
        public static bool CompareEqual(KObject a, KObject b, List<KObject> visited)
        {
            if (compareEq(a, b))
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

