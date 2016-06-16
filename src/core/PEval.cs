using System;

namespace Kernel
{
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
}

