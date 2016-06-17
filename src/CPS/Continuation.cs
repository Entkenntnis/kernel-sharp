using System;
using System.Collections.Generic;

namespace Kernel
{
    public class Continuation<T>
    {
        public Func<T, RecursionResult<T>> NextStep
        {
            get;
            private set;
        }
        public Continuation<T> Parent
        {
            get;
            private set;
        }
        public KObject Context
        {
            get;
            private set;
        }
        //hacky
        public bool isError = false;
        public Continuation(Func<T, RecursionResult<T>> next, Continuation<T> parent, KObject context)
        {
            NextStep = next;
            Parent = parent;
            Context = context;
        }
    }
}

