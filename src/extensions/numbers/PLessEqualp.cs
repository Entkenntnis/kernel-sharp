using System;

namespace Kernel
{
    public class PLessEqualp : POperative
    {
        public PLessEqualp()
        {
        }

        public override string getName()
        {
            return "<=?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return NumbersModule.Numberp(args, (a, b) => {
                return a.LessEqual(b);
            });
        }
    }
}

