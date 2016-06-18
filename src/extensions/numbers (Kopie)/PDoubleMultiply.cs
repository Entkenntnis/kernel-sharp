using System;

namespace Kernel
{
    public class PDoubleMultiply : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("double-multiply", cont, () => {
                PHelper.CheckParameter(args, 2);
                KDouble a = PHelper.First(args) as KDouble;
                KDouble b = PHelper.Second(args) as KDouble;
                PHelper.Check(a);
                PHelper.Check(b);
                return new KDouble(a.Value * b.Value);
            });
        }
    }
}

