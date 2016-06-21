using System;
using System.Collections.Generic;

namespace Kernel
{
    public class Continuation<T>
    {
        public readonly Func<T, RecursionResult<T>> NextStep;
        public readonly Continuation<T> Parent;
        //hacky
        public object Context;
        public bool isError = false;
        public bool isHandler = false;
        public Continuation(Func<T, RecursionResult<T>> next, Continuation<T> parent, object context)
        {
            NextStep = next;
            Parent = parent;
            Context = context;
        }
    }
}

