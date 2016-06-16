using System;
using System.Collections.Generic;

namespace Kernel
{
    public class Continuation<T>
    {
        public Func<T, Continuation<T>, RecursionResult<T>> NextStep
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
        public LinkedList<KObject> _RemainingObjs;
        public LinkedList<KObject> _Pairs;
        public KObject _Placeholder;
        public bool isError = false;
        //public List<KGuard> EntryGuard = null;
        //public List<KGuard> ExitGuard = null;
        public Continuation<KObject> _RemainingGuards;
        public Continuation(Func<T, Continuation<T>, RecursionResult<T>> next, Continuation<T> parent, KObject context)
        {
            NextStep = next;
            Parent = parent;
            Context = context;
        }
    }
}

