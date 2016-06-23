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
    }
}

