using System;
using System.Collections.Generic;
using System.IO;

namespace Kernel
{
    public class PBoolean : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "boolean?", typeof(KBoolean), cont);
        }
    }

    public class PSymbol : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "symbol?", typeof(KSymbol), cont);
        }
    }

    public class PInert : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "inert?", typeof(KInert), cont);
        }
    }
    public class PIgnore : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "ignore?", typeof(KIgnore), cont);
        }
    }
    public class PNull : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "null?", typeof(KNil), cont);
        }
    }
    public class PPair : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "pair?", typeof(KPair), cont);
        }
    }
    public class PEnvironment : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "environment?", typeof(KEnvironment), cont);
        }
    }
    public class POperative : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "operative?", typeof(KOperative), cont);
        }
    }
    public class PApplicative : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "applicative?", typeof(KApplicative), cont);
        }
    }
    public class PContinuation : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "continuation?", typeof(KContinuation), cont);
        }
    }


    public class PSetCar : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "set-car!");
            if (res != null)
                return CPS.Error(res, cont);
            KObject p = First(args), v = Second(args);
            if (p is KPair)
            {
                KPair pair = p as KPair;
                if (pair.Mutable)
                {
                    pair.SetCar(v);
                    return Return(new KInert(), cont);
                }
            }
            return CPS.Error("set-car: pair not mutable", cont);
        }
    }
    public class PSetCdr : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "set-cdr!");
            if (res != null)
                return CPS.Error(res, cont);
            KObject p = First(args), v = Second(args);
            if (p is KPair)
            {
                KPair pair = p as KPair;
                if (pair.Mutable)
                {
                    pair.SetCdr(v);
                    return Return(new KInert(), cont);
                }
            }
            return CPS.Error("set-cdr: pair not mutable", cont);
        }
    }
    public class PCopyEsImmutable : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "copy-es-immutable");
            if (res != null)
                return CPS.Error(res, cont);
            return Return(KPair.CopyEsImmutable(First(args)), cont);
        }
    }
    public class PCopyEs : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "copy-es-immutable");
            if (res != null)
                return CPS.Error(res, cont);
            return Return(KPair.CopyEs(First(args)), cont);
        }
    }


    public class PMakeEnvironment : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            KEnvironment envir = new KEnvironment();
            try
            {
                KPair.Foreach(x => { 
                    if (!(x is KEnvironment))
                        throw new RuntimeException("make-environment: not an environment");
                    envir.AddParent((KEnvironment)x);
                }, args);
            }
            catch (Exception e)
            {
                return CPS.Error(e.Message, cont);
            }
            return Return(envir, cont);
        }
    }


    public class PUnwrap : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "unwrap");
            if (res != null)
                return CPS.Error(res, cont);
            KObject ap = First(args);
            if (!(ap is KApplicative))
                return CPS.Error("unwrap: not an applicative", cont);
            else
                return Return((ap as KApplicative).Combiner, cont);
        }
    }



    public class PWrap : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "wrap");
            if (res != null)
                return CPS.Error(res, cont);
            KObject op = First(args);
            if (!(op is KOperative || op is KApplicative))
                return CPS.Error("wrap: not a operative", cont);
            else
                return Return(new KApplicative(op as KCombiner), cont);
        }
    }

    public class PWrite : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "write");
            if (res != null)
                return CPS.Error(res, cont);
            KObject datum = First(args);
            Console.WriteLine(datum.Write());
            return Return(new KInert(), cont);
        }
    }

    public class PDisplay : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "display");
            if (res != null)
                return CPS.Error(res, cont);
            KObject datum = First(args);
            Console.WriteLine(datum.Display());
            return Return(new KInert(), cont);
        }
    }

    public class PEncapE : KOperative
    {
        private int id;
        public PEncapE(int id)
        {
            this.id = id;
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "encapsulate");
            if (res != null)
                return CPS.Error(res, cont);
            KObject datum = First(args);
            return Return(new KEncapsulation(id, datum), cont);
        }
    }
    public class PEncapP : KOperative
    {
        private int id;
        public PEncapP(int id)
        {
            this.id = id;
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            int len = KPair.Length(args);
            if (len == -1)
                return CPS.Error("encapsulation?: parameter is not a list", cont);
            else
            {
                bool result = true;
                KPair.Foreach(x =>
                    {
                        if (!(x is KEncapsulation) || (x as KEncapsulation).Id != id)
                            result = false;
                    }, args);
                return ReturnBool(result, cont);
            }
        }
    }

    public class PEncapD : KOperative
    {
        private int id;
        public PEncapD(int id)
        {
            this.id = id;
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "decapsle");
            if (res != null)
                return CPS.Error(res, cont);
            KObject datum = First(args);
            if (!(datum is KEncapsulation))
                return CPS.Error("decaplse: not an encap", cont);
            KEncapsulation enc = datum as KEncapsulation;
            if (enc.Id != id)
                return CPS.Error("decaplse: wrong", cont);
            return Return(enc.Value, cont);
        }
    }
    public class PEncap : KOperative
    {
        private static int counter = 0;
        private int id;
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 0, "make-encapsulation-type");
            if (res != null)
                return CPS.Error(res, cont);
            this.id = counter++;
            KApplicative e = new KApplicative(new PEncapE(id));
            KApplicative p = new KApplicative(new PEncapP(id));
            KApplicative d = new KApplicative(new PEncapD(id));
            KPair p3 = new KPair(d, new KNil(), true);
            KPair p2 = new KPair(p, p3, true);
            KPair p1 = new KPair(e, p2, true);
            return Return(p1, cont);
        }
    }

    public class PCallCC : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "call/cc");
            if (res != null)
                return CPS.Error(res, cont);
            KObject comb = First(args);
            try
            {
                if (comb is KCombiner)
                {
                    return (comb as KCombiner).Combine(new KPair(new KContinuation(cont), new KNil(), true), env, cont);
                }
            }
            catch (Exception e)
            {
                return CPS.Error(e.Message, cont);
            }
            return CPS.Error("call/cc: not a combiner", cont);
        }
    }

    public class PContinuationToApplicative : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "continuation->applicative");
            if (res != null)
                return CPS.Error(res, cont);
            KObject c = First(args);
            if (c is KContinuation)
            {
                return CPS.Return(new KApplicative(new PPassCont(c as KContinuation)), cont);
            }
            return CPS.Error("continuation->applicative: not a continuation given", cont);
        }
    }

    public class PPassCont : KOperative
    {
        private KContinuation myc;
        public PPassCont(KContinuation c)
        {
            myc = c;
        }
        private bool continuationEqual(Continuation<KObject> a, Continuation<KObject> b)
        {
            if ((a.Context == null && b.Context == null) || (a.Parent == null && b.Parent == null))
            {
                return true;
            }
            return a.isError == b.isError &&
                a.Context == b.Context &&
            a.NextStep == b.NextStep &&
                PEqual.CompareEqual(a.Context, b.Context, new List<KObject>()) &&
            a.EntryGuard == b.EntryGuard &&
            a.ExitGuard == b.ExitGuard &&
            continuationEqual(a.Parent, b.Parent);
        }

        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            // first of all I wanna check for exit guards
            //Continuation<KObject> loop = cont;
            Continuation<KObject> result = myc.Value;
            /*var conts = new LinkedList<Continuation<KObject>>();
            while (loop != null)
            {
                conts.AddFirst(loop);
                loop = loop.Parent;
            }
            loop = cont;
            Continuation<KObject> innerloop = myc.Value;
            while(true)
            {
                if (innerloop == null)
                    break;
                if (continuationEqual(innerloop, cont))
                {
                    loop = null;
                    break;
                }
                innerloop = innerloop.Parent;
            }
            while (loop != null)
            {
                if (loop.ExitGuard != null && loop.ExitGuard.Count > 0)
                {
                    // ok, found something, now check
                    foreach (KGuard guard in loop.ExitGuard)
                    {
                        foreach (Continuation<KObject> c in conts)
                        {
                            // from the last to the first
                            if (continuationEqual(c, guard.Selector.Value))
                            {
                                var child = new Continuation<KObject>((x, ctxt) =>
                                    {
                                        return Evaluator.rceval(new KPair(guard.Interceptor, new KPair(x, new KPair(new KNil(), new KNil(), true), true), true), KEnvironment.GetGroundEnv(), ctxt._RemainingGuards);
                                    }, result, new KString("guard"));
                                child._RemainingGuards = result;
                                result = child;
                            }
                        }
                    }
                }
                loop = loop.Parent;
            }*/

            if (result.isError)
            {
                Continuation<KObject> cc = new Continuation<KObject>(null, cont, null);
                cc.isError = true;
                return CPS.Return(args, cc);
            }
            if (result.Context == null)
            {
                //well, top level reached hm hm
                return CPS.Return(args, result);
            }
            return result.NextStep(args, result);
        }
    }

    public class PExtendContinuatione : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            int len = KPair.Length(args);
            if (len < 2 || len > 3)
                return CPS.Error("extend-continuation: argument mismatch", cont);
            KContinuation argC = First(args) as KContinuation;
            KApplicative argA = Second(args) as KApplicative;
            KEnvironment argE = len == 3 ? Third(args) as KEnvironment : new KEnvironment();
            if (argC == null || argA == null || argE == null)
                return CPS.Error("extend-continuation: mismatching arguments", cont);

            var nc = new Continuation<KObject>((val, ctxt) =>
                {
                    return CPS.Next(() => Evaluator.rceval(new KPair(argA.Combiner, val, true), argE, argC.Value), argC.Value);
                }, argC.Value, argA);
            return Return(new KContinuation(nc), cont);
        }
    }

    public class PGuardContinuation : KOperative
    {
        private List<KGuard> assignGuards(KObject clauses)
        {
            List<KGuard> result = new List<KGuard>();
            if (clauses is KPair)
            {
                KPair.Foreach(x =>
                    {
                        int length = KPair.Length(x);
                        if (length == 2)
                        {
                            KContinuation selector = First(x) as KContinuation;
                            KApplicative interceptor = Second(x) as KApplicative;
                            if (selector == null || interceptor == null)
                                throw new RuntimeException("guard-continuation: invalid clause, wrong types");
                            result.Add(new KGuard{ Selector = selector, Interceptor = interceptor });
                        }
                        else
                            throw new RuntimeException("guard-continuation: invalid clause");
                    }, clauses);
            }
            return result;
        }

        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 3, "guard-continuation");
            if (res != null)
                return CPS.Error(res, cont);
            KContinuation c = Second(args) as KContinuation;
            KObject entry = First(args);
            KObject exit = Third(args);
            try
            {
                if (null == c)
                    throw new RuntimeException("guard-continuation: not a continution");
                c.Value.EntryGuard = assignGuards(entry);
                c.Value.ExitGuard = assignGuards(exit);
                return CPS.Return(c, cont);
            }
            catch (Exception e)
            {
                return CPS.Error(e.Message, cont);
            }
        }
    }

    public class PLoad : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "load");


            if (res != null)
                return CPS.Error(res, cont);
            KObject val = First(args) as KString;
            if (!(val is KString))
                return CPS.Error("load: not a string", cont);
            string path = (val as KString).Value;
            try
            {
                List<KObject> tokens = Parser.ParseAll(File.ReadAllText(path));
                foreach (var token in tokens)
                {
                    Evaluator.Eval(token, env);
                }
                return CPS.Return(new KInert(), cont);
            }
            catch (Exception e)
            {
                return CPS.Error("Failed to load file: " + e.Message, cont);
            }
        }
    }
}

