using System;

namespace Kernel
{
    public class PInteger2Double : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 1, "integer->double");
            if (null != res)
                return PHelper.Error(res, cont);
            KInteger i = PHelper.First(args) as KInteger;
            if (null == i)
                return PHelper.Error("integer->double: input not integer", cont);
            return PHelper.Return(new KDouble((double)i.Value), cont);
        }
    }
}

