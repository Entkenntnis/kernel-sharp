using System;

namespace Kernel
{
    public class PDefine : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 2, "$define!");
            if (res != null)
                return PHelper.Error(res, cont);
            KObject definand = PHelper.First(args), expr = PHelper.Second(args);
            var cc = new Continuation<KObject>((e) =>
                {
                    try
                    {
                        BindFormalTree(definand, e, env);
                    }
                    catch (Exception ex)
                    {
                        return PHelper.Error(ex.Message, cont);
                    }
                    return PHelper.Return(new KInert(), cont);
                }, cont, expr.Display());
            return CPS.PassTo(() => Evaluator.rceval(expr, env, cc));        
        }
    }
}

