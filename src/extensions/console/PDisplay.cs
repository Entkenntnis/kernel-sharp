using System;

namespace Kernel
{
    public class PDisplay : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return PHelper.Do("display", cont, () => {
                PHelper.CheckParameter(args, 1);
                Console.WriteLine(PHelper.First(args).Display());
                return new KInert();
            });
        }
    }
}

