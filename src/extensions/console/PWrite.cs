using System;

namespace Kernel
{
    public class PWrite : POperative
    {
        public override string getName()
        {
            return "write";
        }
        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            Console.WriteLine(First(args).Write());
            return new KInert();
        }
    }
}

