using System;

namespace Kernel
{
    public class PEncapE : POperative
    {
        private int id;
        public PEncapE(int id)
        {
            this.id = id;
        }

        public override string getName()
        {
            return "<#internal:encapE>";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KObject datum = First(args);
            return new KEncapsulation(id, datum);
        }
    }
}

