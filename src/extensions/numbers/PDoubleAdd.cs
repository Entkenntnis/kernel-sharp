using System;

namespace Kernel
{
    public class PDoubleAdd : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("double-add", cont, () => {
                PHelper.CheckParameter(args, 2);
                KDouble a = PHelper.First(args) as KDouble;
                KDouble b = PHelper.Second(args) as KDouble;
                PHelper.Check(a);
                PHelper.Check(b);
                return new KDouble(a.Value + b.Value);
            });
        }
    }
}

