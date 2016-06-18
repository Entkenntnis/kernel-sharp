using System;

namespace Kernel
{
    public class PEval : POperative
    {
        public override string getName()
        {
            return "eval";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 2);
            KObject expr = First(args);
            KEnvironment envir = Second(args) as KEnvironment;
            Check(envir, "not an enviroment");
            return CPS.PassTo<KObject>(() => Evaluator.rceval(expr, envir, cont));
        }
    }
}

