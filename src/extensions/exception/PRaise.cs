using System;

namespace Kernel
{
    public class PRaise : POperative
    {
        public PRaise()
        {
        }

        public override string getName()
        {
            return "raise";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            Check(First(args), "message not a string");
            return CPS.Error<KObject>((First(args) as KString).Value, cont);
        }
    }
}

