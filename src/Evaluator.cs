﻿using System;
using System.Collections.Generic;

namespace Kernel
{
    
    public class Evaluator
    {
        public static KObject Eval(KObject datum, KEnvironment env)
        {
            return CPS.Execute<KObject>(() => rceval(datum, env, CPS.RootContinuation<KObject>()));
        }

        public static RecursionResult<KObject> rceval(KObject datum, KEnvironment env, Continuation<KObject> cont)
        {
            // useful for debugging
            //Console.WriteLine(datum.Display());

            if (datum is KPair)
            {
                KPair p = datum as KPair;

                // this function get called when the operator is evaluated to f
                var childCont = new Continuation<KObject>((f) =>
                    {
                        if (f is KOperative)
                        {
                            //return ((KOperative)f).Combine(p.Cdr, env, cont);

                             //   
                            return combineOp(f as KOperative, p.Cdr, env, cont);
                        }
                        else if (f is KApplicative && (p.Cdr is KPair || p.Cdr is KNil))
                        {
                            if (p.Cdr is KNil)
                            {
                                KApplicative app = f as KApplicative;
                                while (app.Combiner is KApplicative)
                                    app = app.Combiner as KApplicative;
                                return combineOp(app.Combiner as KOperative, p.Cdr, env, cont);
                            }
                            KPair ops = p.Cdr as KPair;
                            LinkedList<KObject> input = new LinkedList<KObject>();
                            KPair.Foreach(x =>
                                {
                                    input.AddLast(x);
                                }, ops);
                            LinkedList<KObject> pairs = new LinkedList<KObject>();
                            Func<KObject, RecursionResult<KObject>> recursion = null;

                            // this continuation is called with the next argument evaled to x. Place next value
                            recursion = (x) =>
                                {
                                    pairs.AddLast(x);
                                    if (input.Count == 0)
                                    {
                                        // we are finished
                                        KObject output = new KNil();
                                        while(pairs.Count > 0) {
                                            output = new KPair(pairs.Last.Value, output);
                                            pairs.RemoveLast();
                                        }
                                        KApplicative app = f as KApplicative;
                                        while (app.Combiner is KApplicative)
                                            app = app.Combiner as KApplicative;
                                        return combineOp(app.Combiner as KOperative, output, env, cont);
                                    }
                                    else
                                    {
                                        // do something with the next Head argument
                                        KObject next = input.First.Value;
                                        input.RemoveFirst();
                                        var cc2 = new Continuation<KObject>(recursion, cont, p);
                                        return CPS.PassTo(() => rceval(next, env, cc2));
                                    }
                                };
                            KObject next2 = input.First.Value;
                            input.RemoveFirst();
                            var cc = new Continuation<KObject>(recursion, cont, p);
                            return CPS.PassTo(() => rceval(next2, env, cc));
                        }
                        return CPS.Error("Unsuitable operation", cont);
                    }, cont, p.Car);
                return CPS.PassTo(() => rceval(p.Car, env, childCont));
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

        private static RecursionResult<KObject> combineOp(KOperative op, KObject operands, KEnvironment env, Continuation<KObject> cont)
        {
            if (null == op.Expr) {
                if (op is POperative) {
                    return (op as POperative).Combine(operands, env, cont);
                }
                return CPS.Error("Primitive without implementation!", cont);
            }
            KEnvironment local = new KEnvironment(op.staticenv);
            if (!(op.EFormal is KIgnore))
                local.Bind(((KSymbol)op.EFormal).Value, env);
            KOperative.BindFormalTree(op.Formals, operands, local);
            return CPS.PassTo<KObject>(() => Evaluator.rceval(op.Expr, local, cont));
        }
    }
}