using System;

namespace Kernel
{
    public class PLessp : POperative
    {
        public PLessp()
        {
        }

        public override string getName()
        {
            return "<?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return NumbersModule.Numberp(args, (a, b) => {
                return a.LessThan(b);
            });
        }
    }
}

