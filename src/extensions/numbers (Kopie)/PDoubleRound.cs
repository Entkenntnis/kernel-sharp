using System;

namespace Kernel
{
    public class PDoubleRound : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("double-round", cont, () => {
                PHelper.CheckParameter(args, 1);
                KDouble a = PHelper.First(args) as KDouble;
                PHelper.Check(a);
                return new KDouble(Math.Round(a.Value));
            });
        }
    }
}

