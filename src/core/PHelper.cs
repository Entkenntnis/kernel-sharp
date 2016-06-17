using System;

namespace Kernel
{
    public class PHelper
    {
        public static RecursionResult<KObject> Return(KObject obj, Continuation<KObject> cont)
        {
            return CPS.Return(obj, cont);
        }
        public static RecursionResult<KObject> ReturnBool(bool b, Continuation<KObject> cont)
        {
            return Return(new KBoolean(b), cont);
        }
        public static string CheckParameter(KObject p, int len, string name)
        {
            int actLen = KPair.Length(p);
            if (actLen != len)
            {
                return name + ": mismatching number of ops, expected " + len + ", got " + actLen;
            }
            return null;
        }
        public static KObject First(KObject p)
        {
            return ((KPair)p).Car;
        }
        public static KObject Second(KObject p)
        {
            return ((KPair)((KPair)p).Cdr).Car;
        }
        public static KObject Third(KObject p)
        {
            return (((KPair)((KPair)p).Cdr).Cdr as KPair).Car;
        }

        public static RecursionResult<KObject> Error(string message, Continuation<KObject> cont)
        {
            return CPS.Error<KObject>(message, cont);
        }
    }
}

