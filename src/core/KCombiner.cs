using System;
using System.Collections.Generic;

namespace Kernel
{
    public abstract class KCombiner : KObject
    {
        public abstract RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont);
    }
}

