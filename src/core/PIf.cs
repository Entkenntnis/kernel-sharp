using System;

namespace Kernel
{
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
}

