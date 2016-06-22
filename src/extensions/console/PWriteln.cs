using System;

namespace Kernel
{
    public class PWriteln : POperative
    {
        public override string getName()
        {
            return "writeln";
        }
        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            Console.WriteLine(First(args).Write());
            return new KInert();
        }
    }
}

