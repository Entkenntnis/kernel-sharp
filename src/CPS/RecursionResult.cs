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
        private readonly RecursionType _type;
        private readonly T _result;
        private readonly Func<RecursionResult<T>> _nextStep;
        private readonly Continuation<T> _cont;
        internal RecursionResult(RecursionType  type, T result, Func<RecursionResult<T>> nextStep, Continuation<T> cont)
        {
            _type = type;
            _result = result;
            _nextStep = nextStep;
            _cont = cont;
        }

        public RecursionType Type { get { return _type; } }
        public T Result { get { return _result; } }
        public Func<RecursionResult<T>> NextStep { get { return _nextStep; } }
        public Continuation<T> Cont { get { return _cont; } }
    }
}

