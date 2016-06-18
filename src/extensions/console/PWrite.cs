using System;

namespace Kernel
{
    public class PWrite : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("write", cont, () => {
                PHelper.CheckParameter(args, 1);
                Console.WriteLine(PHelper.First(args).Write());
                return new KInert();
            });
        }
    }
}

