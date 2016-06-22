using System;

namespace Kernel
{
    public class PPassCont : POperative
    {
        private KContinuation myc;
        public PPassCont(KContinuation c)
        {
            myc = c;
        }

        public override string getName()
        {
            return "#<internal:passcont>";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            Continuation<KObject> result = myc.Value;
            if (result.Context == null)
                return CPS.Return(First(args), CPS.RootContinuation<KObject>());
            return result.Call(First(args));
        }

        /*public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            
            Continuation<KObject> result = myc.Value;

            if (result.isError)
            {
                Continuation<KObject> cc = new Continuation<KObject>(null, cont, null);
                cc.isError = true;
                return CPS.Return(args, cc);
            }
            if (result.Context == null)
            {
                //well, top level reached hm hm
                return CPS.Return(args, result);
            }
            return result.NextStep(args, result);
        }*/
    }
}

