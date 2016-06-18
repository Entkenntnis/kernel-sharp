using System;

namespace Kernel
{
    public class PIf : POperative
    {

        public override string getName()
        {
            return "$if";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 3);
            KObject pred = First(args), tr = Second(args), fl = Third(args);
            var cc = new Continuation<KObject>((p) => {
                if (!(p is KBoolean))
                    return CPS.Error<KObject>("$if: predicate not boolean", cont);
                else {
                    if (((KBoolean)p).Value)
                        return CPS.PassTo<KObject>(() => Evaluator.rceval(tr, env, cont));
                    else
                        return CPS.PassTo<KObject>(() => Evaluator.rceval(fl, env, cont));
                }
            }, cont, pred.Display());
            return CPS.PassTo(() => Evaluator.rceval(pred, env, cc));   
        }
    }
}

