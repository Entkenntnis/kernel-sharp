using System;

namespace Kernel
{
    public class PVau : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 3, "$vau");
            if (res != null)
                return PHelper.Error(res, cont);
            try
            {
                KObject formalt = PHelper.First(args), eformal = PHelper.Second(args), expr = PHelper.Third(args);
                KOperative op = new KOperative(formalt, eformal, expr, env);
                return PHelper.Return(op, cont);
            }
            catch (Exception e)
            {
                return PHelper.Error(e.Message, cont);
            }
        }
    }
}

