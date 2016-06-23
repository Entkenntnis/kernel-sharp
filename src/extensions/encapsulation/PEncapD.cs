using System;

namespace Kernel
{
    public class PEncapD : POperative
    {
        private int id;
        public PEncapD(int id)
        {
            this.id = id;
        }

        public override string getName()
        {
            return "<#internal:encapD>";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KEncapsulation datum = First(args) as KEncapsulation;
            Check(datum, "not an encap");
            if (datum.Id != id)
                throw new RuntimeException("wrong id");
            return datum.Value;
        }
    }
}

