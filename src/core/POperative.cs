using System;

namespace Kernel
{
    public abstract class POperative : KOperative
    {
        public abstract RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont);
    }
}

