using System;

namespace Kernel
{
    public class PRead : POperative
    {
        public PRead()
        {
        }

        public override string getName()
        {
            return "read";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 0);
            KObject datum = null;
            while(null == datum)
                datum = Interpreter.readDatum();
            return datum;
        }
    }
}

