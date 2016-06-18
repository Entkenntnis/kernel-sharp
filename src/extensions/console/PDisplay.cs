using System;

namespace Kernel
{
    public class PDisplay : POperative
    {
        public override string getName()
        {
            return "display";
        }
        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            Console.WriteLine(First(args).Display());
            return new KInert();
        }
    }
}

