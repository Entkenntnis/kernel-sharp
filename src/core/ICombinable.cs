using System;

namespace Kernel
{
    public interface ICombinable
    {
        RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont);
    }
}

