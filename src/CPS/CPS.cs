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

        public static RecursionResult<KObject> Error(KObject message, Continuation<KObject> cont)
        {
            Continuation<KObject> cc = new Continuation<KObject>((x) =>
                {
                    return null;
                }, cont, null);
            cc.isError = true;
            return new RecursionResult<KObject>(RecursionType.Return, message, null, cc);
        }

        public static RecursionResult<KObject> Error(string message, Continuation<KObject> cont)
        {
            return CPS.Error(new KSymbol(message), cont);
        }

        public static Continuation<T> RootContinuation<T>()
        {
            return new Continuation<T>(null, null, null);
        }

    }




}

