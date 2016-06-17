using System;

namespace Kernel
{
    public class PEval : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 2, "eval");
            if (res != null)
                return PHelper.Error(res, cont);
            KObject expr = PHelper.First(args), envir = PHelper.Second(args);
            if (!(envir is KEnvironment))
                return PHelper.Error("eval: not an environment", cont);
            return CPS.PassTo<KObject>(() => Evaluator.rceval(expr, (KEnvironment)envir, cont));
        }
    }
}

