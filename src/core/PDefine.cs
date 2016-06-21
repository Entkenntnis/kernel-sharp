using System;

namespace Kernel
{
    public class PDefine : POperative
    {
        public override string getName()
        {
            return "$define!";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 2);
            KObject definand = First(args), expr = Second(args);
            var cc = new Continuation<KObject>((e) => {
                try {
                    Evaluator.BindFormalTree(definand, e, env);
                } catch (Exception ex) {
                    return CPS.Error<KObject>(ex.Message, cont);
                }
                return CPS.Return<KObject>(new KInert(), cont);
            }, cont, "define");
            return CPS.PassTo(() => Evaluator.rceval(expr, env, cc)); 
        }
    }
}

