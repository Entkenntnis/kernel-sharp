using System;

namespace Kernel
{
    public class PCallCC : POperative
    {
        public override string getName()
        {
            return "call/cc";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KCombiner comb = First(args) as KCombiner;
            Check(comb, "not a combiner");
            return Evaluator.rceval(new KPair(comb, new KPair(new KContinuation(cont), new KNil())), env, cont);
        }
    }
}

