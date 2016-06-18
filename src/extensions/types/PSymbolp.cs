using System;

namespace Kernel
{
    public class PSymbolp : POperative
    {
        public PSymbolp()
        {
        }

        public override string getName()
        {
            return "symbol?";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            return First(args) is KSymbol;
        }
    }
}

