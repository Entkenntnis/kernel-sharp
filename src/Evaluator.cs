using System;
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
                            return combineOp(f as KOperative, p.Cdr, env, cont);
                        }
                        else if (f is KApplicative && (p.Cdr is KPair || p.Cdr is KNil))
                        {
                            if (p.Cdr is KNil)
                            {
                                return combineApp(f as KApplicative, p.Cdr, env, cont);
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
                                        return combineApp(f as KApplicative, output, env, cont);
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
                            var cc = new Continuation<KObject>(recursion, cont, p.Display());
                            return CPS.PassTo(() => rceval(next2, env, cc));
                        }
                        return CPS.Error<KObject>("Unsuitable operation of " + f.Write(), cont);
                    }, cont, p.Car.Display());
                return CPS.PassTo(() => rceval(p.Car, env, childCont));
            }
            else if (datum is KSymbol)
            {
                KObject val = env.Lookup(((KSymbol)datum).Value);
                if (null == val)
                    return CPS.Error<KObject>("Unbound variable " + ((KSymbol)datum).Value, cont);
                return CPS.Return(val, cont);
            }
            else
                return CPS.Return(datum, cont);
        }

        private static RecursionResult<KObject> combineApp(KApplicative app, KObject operands, KEnvironment env, Continuation<KObject> cont)
        {
            while (app.Combiner is KApplicative)
                app = app.Combiner as KApplicative;
            return combineOp(app.Combiner as KOperative, operands, env, cont);
        }

        private static RecursionResult<KObject> combineOp(KOperative op, KObject operands, KEnvironment env, Continuation<KObject> cont)
        {
            if (null == op.Expr) {
                if (op is ICombinable) {
                    return (op as ICombinable).Combine(operands, env, cont);
                }
                return CPS.Error<KObject>("Primitive without implementation!" + op.Write(), cont);
            }
            KEnvironment local = new KEnvironment(op.staticenv);
            if (!(op.EFormal is KIgnore))
                local.Bind(((KSymbol)op.EFormal).Value, env);
            BindFormalTree(op.Formals, operands, local);
            return CPS.PassTo<KObject>(() => Evaluator.rceval(op.Expr, local, cont));
        }


        public static void BindFormalTree(KObject formal, KObject vals, KEnvironment env, KObject ctxt = null)
        {
            if (ctxt == null)
                ctxt = formal;
            if (formal is KSymbol)
            {
                env.Bind(((KSymbol)formal).Value, vals);
            }
            else if (formal is KIgnore)
                return;
            else if (formal is KNil && vals is KNil)
                return;
            else if (formal is KPair && vals is KPair)
            {
                KPair f = formal as KPair;
                KPair v = vals as KPair;
                BindFormalTree(f.Car, v.Car, env, ctxt);
                BindFormalTree(f.Cdr, v.Cdr, env, ctxt);
            }
            else
                    throw new RuntimeException("Can't bind formal tree of " + ctxt.Write());
        }
    }
}