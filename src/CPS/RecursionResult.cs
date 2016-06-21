using System;

namespace Kernel
{
    public enum RecursionType
    {
        Return,
        Next
    }

    public class RecursionResult<T>
    {
        public readonly RecursionType Type;
        public readonly T Result;
        public readonly Func<RecursionResult<T>> NextStep;
        public readonly Continuation<T> Cont;
        internal RecursionResult(RecursionType  type, T result, Func<RecursionResult<T>> nextStep, Continuation<T> cont)
        {
            Type = type;
            Result = result;
            NextStep = nextStep;
            Cont = cont;
        }
    }
}

