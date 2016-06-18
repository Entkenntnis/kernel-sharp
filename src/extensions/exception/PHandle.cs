using System;

namespace Kernel
{
    public class PHandle : POperative
    {
        public override string getName()
        {
            return "$handle";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 2);
            KPair p = args as KPair;
            Continuation<KObject> c = null;
            c = new Continuation<KObject>((x) => {
                return CPS.PassTo(() => Evaluator.rceval(
                    new KPair(p.Car, new KPair(new KString(c.Context.ToString()), new KNil())), env, cont));
            }, cont, "error-handler");
            c.isHandler = true;
            return CPS.PassTo(() => Evaluator.rceval(Second(p), env, c));
        }
    }
}

