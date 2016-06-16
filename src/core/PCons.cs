using System;

namespace Kernel
{
    public class PCons : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "cons");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args);
            KObject b = Second(args);
            return Return(new KPair(a, b, true), cont);
        }
    }
}

