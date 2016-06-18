using System;

namespace Kernel
{
    public class PIntegerDivide : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("integer-divide", cont, () => {
                PHelper.CheckParameter(args, 2);
                KInteger a = PHelper.First(args) as KInteger;
                KInteger b = PHelper.Second(args) as KInteger;
                PHelper.Check(a);
                PHelper.Check(b);
                if (b.Value == 0)
                    throw new RuntimeException("division by zero");
                return new KInteger(a.Value / b.Value);
            });
        }
    }
}

