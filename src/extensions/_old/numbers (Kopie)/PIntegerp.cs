using System;

namespace Kernel
{
    public class PIntegerp : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 1, "integer?");
            if (res != null)
                return PHelper.Error(res, cont);
            return PHelper.ReturnBool(PHelper.First(args) is KInteger, cont);
        }
    }
}

