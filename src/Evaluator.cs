﻿using System;
using System.Collections.Generic;

namespace Kernel
{
    public class RuntimeException : Exception
    {
        public RuntimeException(string message) : base(message){ }
    }
    public class Evaluator
    {
        public static KObject Eval(KObject datum, KEnvironment env)
        {
            return CPS.Execute<KObject>(() => rceval(datum, env, CPS.RootContinuation<KObject>()));
        }
        private static LinkedList<KObject> CopyLL(LinkedList<KObject> orig)
        {
            LinkedList<KObject> newLL = new LinkedList<KObject>();
            foreach (KObject obj in orig)
            {
                newLL.AddLast(obj);
            }
            return newLL;
        }

        public static RecursionResult<KObject> rceval(KObject datum, KEnvironment env, Continuation<KObject> cont)
        {
            // useful for debugging
            //Console.WriteLine(datum.Display());

            if (datum is KPair)
            {
                KPair p = datum as KPair;

                // this function get called when the operator is evaluated to f
                var childCont = new Continuation<KObject>((f, unused) =>
                    {
                        if (f is KOperative)
                        {
                            return ((KOperative)f).Combine(p.Cdr, env, cont);
                        }
                        else if (f is KApplicative && (p.Cdr is KPair || p.Cdr is KNil))
                        {
                            if (p.Cdr is KNil)
                            {
                                return ((KApplicative)f).Combine(p.Cdr, env, cont);
                            }
                            KPair ops = p.Cdr as KPair;
                            LinkedList<KObject> input = new LinkedList<KObject>();
                            KObject placeholder = KPair.Map(x =>
                                {
                                    input.AddLast(x);
                                    return new KInert();
                                }, ops);
                            LinkedList<KObject> pairs = new LinkedList<KObject>();
                            Func<KObject, Continuation<KObject>,  RecursionResult<KObject>> recursion = null;

                            // this continuation is called with the next argument evaled to x. Place next value
                            recursion = (x, ctxt) =>
                                {
                                    if (input == null)
                                    {
                                        input = CopyLL(ctxt._RemainingObjs);
                                        placeholder = ctxt._Placeholder;
                                        pairs = CopyLL(ctxt._Pairs);
                                    }
                                    pairs.AddLast(x);
                                    if (input.Count == 0)
                                    {
                                        // we are finished
                                        KObject structure = KPair.Map(z =>
                                            {
                                                KObject t = pairs.First.Value;
                                                pairs.RemoveFirst();
                                                return t;
                                            }, placeholder);
                                        input = null;
                                        return ((KApplicative)f).Combine(structure, env, cont);
                                    }
                                    else
                                    {
                                        // do something with the next Head argument
                                        KObject next = input.First.Value;
                                        input.RemoveFirst();
                                        var cc2 = new Continuation<KObject>(recursion, cont, p);
                                        cc2._Placeholder = placeholder;
                                        cc2._RemainingObjs = input;
                                        cc2._Pairs = pairs;
                                        return CPS.Next(() => rceval(next, env, cc2), cc2);
                                    }
                                };
                            KObject next2 = input.First.Value;
                            input.RemoveFirst();
                            var cc = new Continuation<KObject>(recursion, cont, p);
                            cc._Placeholder = placeholder;
                            cc._RemainingObjs = input;
                            cc._Pairs = pairs;
                            return CPS.Next(() => rceval(next2, env, cc), cc);
                        }
                        return CPS.Error("Unsuitable operation", cont);
                    }, cont, p.Car);
                return CPS.Next(() => rceval(p.Car, env, childCont), childCont);
            }
            else if (datum is KSymbol)
            {
                KObject val = env.Lookup(((KSymbol)datum).Value);
                if (null == val)
                    return CPS.Error("Unbound variable " + ((KSymbol)datum).Value, cont);
                return CPS.Return(val, cont);
            }
            else
                return CPS.Return(datum, cont);
        }
    }
}