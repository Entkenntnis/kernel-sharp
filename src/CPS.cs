using System;
using System.Collections.Generic;
using System.Text;

namespace Kernel
{
    public static class CPS
    {
        public static T Execute<T>(Func<RecursionResult<T>> func)
        {
            var recursionResult = func();
            do
            {
                if (recursionResult.Cont.isError)
                {
                    string message =  recursionResult.Result is KObject ? (recursionResult.Result as KObject).Display() : recursionResult.ToString();
                    // get call stack!
                    StringBuilder cs = new StringBuilder();
                    cs.AppendLine("Backtrace:");
                    var cc = recursionResult.Cont.Parent;
                    while (cc.Context != null)
                    {
                        cs.AppendLine(cc.Context.Display());
                        cc = cc.Parent;
                    }
                    cs.Append("------------");
                    throw new RuntimeException(message + "\n" + cs.ToString());
                }
                if (recursionResult.Type == RecursionType.Return)
                {
                    if (recursionResult.Cont.Context == null)
                    {
                        
                        return recursionResult.Result;
                    }
                    else
                    {
                        // pops one level from the stack
                        recursionResult = recursionResult.Cont.NextStep(recursionResult.Result, recursionResult.Cont);
                    }
                }
                else
                {
                    recursionResult = recursionResult.NextStep();
                }

            } while (true);
        }

        public static RecursionResult<T> Return<T>(T result, Continuation<T> cont)
        {
            return new RecursionResult<T>(RecursionType.Return, result, null, cont);
        }

        public static RecursionResult<T> Next<T>(Func<RecursionResult<T>> nextStep, Continuation<T> cont)
        {
            return new RecursionResult<T>(RecursionType.Next, default(T), nextStep, cont);
        }

        public static RecursionResult<KObject> Error(KObject message, Continuation<KObject> cont)
        {
            Continuation<KObject> cc = new Continuation<KObject>((x, ctct) =>
                {
                    return null;
                }, cont, null);
            cc.isError = true;
            return new RecursionResult<KObject>(RecursionType.Return, message, null, cc);
        }

        public static RecursionResult<KObject> Error(string message, Continuation<KObject> cont)
        {
            return CPS.Error(new KString(message), cont);
        }

        public static Continuation<T> RootContinuation<T>()
        {
            return new Continuation<T>(null, null, null);
        }

    }

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
        public List<KGuard> EntryGuard = null;
        public List<KGuard> ExitGuard = null;
        public Continuation<KObject> _RemainingGuards;
        public Continuation(Func<T, Continuation<T>, RecursionResult<T>> next, Continuation<T> parent, KObject context)
        {
            NextStep = next;
            Parent = parent;
            Context = context;
        }
    }
}

