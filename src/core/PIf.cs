using System;

namespace Kernel
{
    public class PIf : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 3, "$if");
            if (res != null)
                return CPS.Error(res, cont);
            KObject pred = PHelper.First(args), tr = PHelper.Second(args), fl = PHelper.Third(args);
            var cc = new Continuation<KObject>((p) =>
                {
                    if (!(p is KBoolean))
                        return CPS.Error("$if: predicate not boolean", cont);
                    else
                    {
                        if (((KBoolean)p).Value)
                            return CPS.PassTo<KObject>(() => Evaluator.rceval(tr, env, cont));
                        else
                            return CPS.PassTo<KObject>(() => Evaluator.rceval(fl, env, cont));
                    }
                }, cont, pred);
            return CPS.PassTo(() => Evaluator.rceval(pred, env, cc));            
        }
    }
}

