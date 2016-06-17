using System;
using System.Collections.Generic;

namespace Kernel
{
    public class KOperative : KCombiner
    {
        public KObject Formals { get; private set; }
        public KObject EFormal{ get; private set; }
        public KObject Expr{ get; private set; }
        public KEnvironment staticenv{ get; private set; }
        public KOperative()
        {
        }
        public KOperative(KObject formals, KObject eformal, KObject expr, KEnvironment env)
        {
            Formals = formals;
            EFormal = eformal;
            Expr = expr;
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

        public override string Print(bool quoteStrings)
        {
            return "#<operative" + (null == Expr ? " p:" + this.GetType().ToString() : "") + ">";
        }


        public List<string> CheckFormalTree(KObject formaltree)
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
        public void BindFormalTree(KObject formal, KObject vals, KEnvironment env)
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

        public override bool CompareTo(KObject other)
        {
            return other is KOperative && other == this;
        }
    }
}

