using System;
using System.Collections.Generic;

namespace Kernel
{
    public class PEqual : KOperative
    {
        public static bool compareEq(KObject a, KObject b)
        {
            if (a.GetType().Equals(b.GetType()))
            {
                if (a is KIgnore)
                    return true;
                else if (a is KInert)
                    return true;
                else if (a is KNil)
                    return true;
                else if (a is KIgnore)
                    return true;
                if (a is KBoolean)
                    return ((KBoolean)a).Value == ((KBoolean)b).Value;
                else if (a is KSymbol)
                    return ((KSymbol)a).Value.Equals(((KSymbol)b).Value);
                /*else if (a is KInteger)
                    return ((KInteger)a).Value == ((KInteger)b).Value;
                else if (a is KDouble)
                    return((KDouble)a).Value == ((KDouble)b).Value;
                else if (a is KString)
                    return ((KString)a).Value.Equals(((KString)b).Value);*/
                else if (a is KPair)
                    return a == b;
                else if (a is KEnvironment)
                    return a == b;
                else if (a is KOperative)
                    return a == b;
                else if (a is KApplicative)
                    return a == b;
                /*else if (a is KEncapsulation)
                    return a == b;
                else if (a is KContinuation)
                    return a == b;*/
            }
            return false;
        }
        public static bool CompareEqual(KObject a, KObject b, List<KObject> visited)
        {
            if (compareEq(a, b))
                return true;
            if (visited.Contains(a) && visited.Contains(b))
                return true;
            else
            {
                if (a is KPair && b is KPair)
                {
                    if (!visited.Contains(a))
                        visited.Add(a);
                    if (!visited.Contains(b))
                        visited.Add(b);
                    KPair ap = a as KPair;
                    KPair bp = b as KPair;
                    return (CompareEqual(ap.Car, bp.Car, visited) && CompareEqual(ap.Cdr, bp.Cdr, visited));
                }
            }
            return false;
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "equal?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args);
            KObject b = Second(args);
            return ReturnBool(CompareEqual(a, b, new List<KObject>()), cont);
        }
    }

    public class PEq : KOperative
    {
        
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "eq?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args);
            KObject b = Second(args);
            return ReturnBool(PEqual.compareEq(a, b), cont);
        }
    }

    public class PCons : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "cons");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args);
            KObject b = Second(args);
            return Return(new KPair(a, b, true), cont);
        }
    }

    public class PEval : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "eval");
            if (res != null)
                return CPS.Error(res, cont);
            KObject expr = First(args), envir = Second(args);
            if (!(envir is KEnvironment))
                return CPS.Error("eval: not an environment", cont);
            return CPS.Next<KObject>(() => Evaluator.rceval(expr, (KEnvironment)envir, cont), cont);
        }
    }

    public class PIf : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 3, "$if");
            if (res != null)
                return CPS.Error(res, cont);
            KObject pred = First(args), tr = Second(args), fl = Third(args);
            var cc = new Continuation<KObject>((p, ctxt) =>
                {
                    if (!(p is KBoolean))
                        return CPS.Error("$if: predicate not boolean", cont);
                    else
                    {
                        if (((KBoolean)p).Value)
                            return CPS.Next<KObject>(() => Evaluator.rceval(tr, env, cont), cont);
                        else
                            return CPS.Next<KObject>(() => Evaluator.rceval(fl, env, cont), cont);
                    }
                }, cont, pred);
            return CPS.Next(() => Evaluator.rceval(pred, env, cc), cc);            
        }
    }

    public class PDefine : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "$define!");
            if (res != null)
                return CPS.Error(res, cont);
            KObject definand = First(args), expr = Second(args);
            var cc = new Continuation<KObject>((e, ctxt) =>
                {
                    try
                    {
                        BindFormalTree(definand, e, env);
                    }
                    catch (Exception ex)
                    {
                        return CPS.Error(ex.Message, cont);
                    }
                    return Return(new KInert(), cont);
                }, cont, expr);
            return CPS.Next(() => Evaluator.rceval(expr, env, cc), cc);        
        }
    }

    public class PVau : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 3, "$vau");
            if (res != null)
                return CPS.Error(res, cont);
            try
            {
                KObject formalt = First(args), eformal = Second(args), expr = Third(args);
                KOperative op = new KOperative(formalt, eformal, expr, env);
                return Return(op, cont);
            }
            catch (Exception e)
            {
                return CPS.Error(e.Message, cont);
            }
        }
    }
}

