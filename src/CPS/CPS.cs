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
                if (recursionResult.Cont != null && recursionResult.Cont.isError)
                {
                    string message =  recursionResult.Cont.Context.ToString();
                    var c = recursionResult.Cont;
                    while (c != null && c.Parent != null && !c.isHandler)
                        c = c.Parent;
                    if (c == null || c.NextStep == null)
                        throw new RuntimeException(message + "\n");
                    else {
                        c.Context = message;
                        recursionResult = c.NextStep(default(T));
                    }
                }
                if (recursionResult.Type == RecursionType.Return)
                {
                    var nextC = recursionResult.Cont;
                    while (nextC.isHandler)
                        nextC = nextC.Parent;
                    if (nextC.Context == null)
                    {
                        
                        return recursionResult.Result;
                    }
                    else
                    {
                        // pops one level from the stack
                        recursionResult = nextC.NextStep(recursionResult.Result);
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

        public static RecursionResult<T> PassTo<T>(Func<RecursionResult<T>> nextStep)
        {
            return new RecursionResult<T>(RecursionType.Next, default(T), nextStep, null);
        }

        public static RecursionResult<T> Error<T>(string message, Continuation<T> cont)
        {
            Continuation<T> cc = new Continuation<T>((x) =>
                {
                    return null;
                }, cont, message);
            cc.isError = true;
            return new RecursionResult<T>(RecursionType.Return, default(T), null, cc);
        }

        public static Continuation<T> RootContinuation<T>()
        {
            return new Continuation<T>(null, null, null);
        }

    }
}

