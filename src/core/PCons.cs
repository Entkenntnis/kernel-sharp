using System;

namespace Kernel
{
    public class PCons : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 2, "cons");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = PHelper.First(args);
            KObject b = PHelper.Second(args);
            return PHelper.Return(new KPair(a, b), cont);
        }
    }
}

