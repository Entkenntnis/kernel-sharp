using System;
using System.Collections.Generic;

namespace Kernel
{
    public abstract class KCombiner : KObject
    {
        public abstract RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont);
    }

    public class KApplicative : KCombiner
    {
        public KCombiner Combiner
        {
            get;
            private set;
        }
        public KApplicative(KCombiner op)
        {
            Combiner = op;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<applicative:" + Combiner.Print(quoteStrings) + ">";
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Combiner.Combine(args, env, cont);
        }
    }

    public class KOperative : KCombiner
    {
        private KObject Formals;
        private KObject EFormal;
        private KObject Expr;
        private KEnvironment staticenv;
        public KOperative()
        {
        }
        public KOperative(KObject formals, KObject eformal, KObject expr, KEnvironment env)
        {
            Formals = KPair.CopyEsImmutable(formals);
            EFormal = eformal;
            Expr = KPair.CopyEsImmutable(expr);
            if (!(eformal is KIgnore || eformal is KSymbol))
                throw new RuntimeException("Can't construct operative, type mismatch of eformal");
            var lst = CheckFormalTree(Formals);
            if (eformal is KSymbol)
            {
                KSymbol s = eformal as KSymbol;
                if (lst.Contains(s.Value))
                    throw new RuntimeException("Distinct eformal needed");
            }
            if (!(env is KEnvironment))
                throw new RuntimeException("Operative: not an environment!");
            else
                staticenv = env;
        }
        protected List<string> CheckFormalTree(KObject formaltree)
        {
            var lst = new List<string>();
            if (formaltree is KNil || formaltree is KIgnore)
                return lst;
            else if (formaltree is KSymbol)
            {
                lst.Add(((KSymbol)formaltree).Value);
                return lst;
            }
            else if (formaltree is KPair)
            {
                KPair p = formaltree as KPair;
                lst.AddRange(CheckFormalTree(p.Car));
                lst.AddRange(CheckFormalTree(p.Cdr));
                return lst;
            }
            else
                throw new RuntimeException("Invalid formal tree");
        }
        public override string Print(bool quoteStrings)
        {
            return "#<operative" + (null == Expr ? " p:" + this.GetType().ToString() : "") + ">";
        }
        protected void BindFormalTree(KObject formal, KObject vals, KEnvironment env)
        {
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
                BindFormalTree(f.Car, v.Car, env);
                BindFormalTree(f.Cdr, v.Cdr, env);
            }
            else
                throw new RuntimeException("Can't bind formal tree!");
        }
        public override RecursionResult<KObject> Combine(KObject operands, KEnvironment env, Continuation<KObject> cont)
        {
            if (null == Expr)
                return CPS.Error("Primitive without implementation!", cont);
            KEnvironment local = new KEnvironment(staticenv);
            if (!(EFormal is KIgnore))
                local.Bind(((KSymbol)EFormal).Value, env);
            BindFormalTree(Formals, operands, local);
            return CPS.Next<KObject>(() => Evaluator.rceval(Expr, local, cont), cont);
        }
        protected RecursionResult<KObject> Return(KObject obj, Continuation<KObject> cont)
        {
            return CPS.Return(obj, cont);
        }
        protected RecursionResult<KObject> ReturnBool(bool b, Continuation<KObject> cont)
        {
            return Return(new KBoolean(b), cont);
        }
        protected string CheckParameter(KObject p, int len, string name)
        {
            int actLen = KPair.Length(p);
            if (actLen != len)
            {
                return name + ": mismatching number of ops, expected " + len + ", got " + actLen;
            }
            return null;
        }
        protected KObject First(KObject p)
        {
            return ((KPair)p).Car;
        }
        protected KObject Second(KObject p)
        {
            return ((KPair)((KPair)p).Cdr).Car;
        }
        protected KObject Third(KObject p)
        {
            return (((KPair)((KPair)p).Cdr).Cdr as KPair).Car;
        }
        protected RecursionResult<KObject> Predicate(KObject args, string name, Type t, Continuation<KObject> cont)
        {
            int len = KPair.Length(args);
            if (len == -1)
                return CPS.Error(name + ": parameter is not a list", cont);
            else
            {
                bool result = true;
                try
                {
                KPair.Foreach(x =>
                    {
                        if (!x.GetType().Equals(t))
                            result = false;
                    }, args);
                }
                catch (Exception e)
                {
                    return CPS.Error(e.Message, cont);
                }
                return ReturnBool(result, cont);
            }
        }
    }
}

