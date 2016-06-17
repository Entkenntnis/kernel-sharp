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
                    throw new RuntimeException("\n" + message + "\n");
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
                        recursionResult = recursionResult.Cont.NextStep(recursionResult.Result);
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

