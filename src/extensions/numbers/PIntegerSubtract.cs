using System;

namespace Kernel
{
    public class PIntegerSubtract : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("integer-subtract", cont, () => {
                PHelper.CheckParameter(args, 2);
                KInteger a = PHelper.First(args) as KInteger;
                KInteger b = PHelper.Second(args) as KInteger;
                PHelper.Check(a);
                PHelper.Check(b);
                return new KInteger(a.Value - b.Value);
            });
        }
    }
}

