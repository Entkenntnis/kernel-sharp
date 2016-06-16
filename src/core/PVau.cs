using System;

namespace Kernel
{
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

