using System;
using System.Collections.Generic;
using System.Text;

namespace Kernel
{
    public static class CPS
    {
        private static object ctxt = null;
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
                    if (c == null || c.Call == null)
                        throw new RuntimeException(message + "\n");
                    else {
                        c.Context = message;
                        setContext(c.Context);
                        recursionResult = c.Call(default(T));
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
                        // pops one level from the stack - we pass result to next continuation
                        setContext(nextC.Context);
                        recursionResult = nextC.Call(recursionResult.Result);
                    }
                }
                else
                {
                    recursionResult = recursionResult.NextStep(); //<- this call is currently always to rceval
                }

            } while (true);
        }
        public static object getContext()
        {
            return ctxt;
        }
        public static void setContext(object obj)
        {
            ctxt = obj;
        }
        public static RecursionResult<T> Return<T>(T result, Continuation<T> cont)
        {
            return new RecursionResult<T>(RecursionType.Return, result, null, cont);
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

