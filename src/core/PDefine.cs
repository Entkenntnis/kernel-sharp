using System;

namespace Kernel
{
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
}

