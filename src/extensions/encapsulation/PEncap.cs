using System;

namespace Kernel
{
    public class PEncap : POperative
    {
        private static int counter = 0;
        private int id;

        public override string getName()
        {
            return "make-encapsulation-type";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 0);
            this.id = counter++;
            KApplicative e = new KApplicative(new PEncapE(id));
            KApplicative p = new KApplicative(new PEncapP(id));
            KApplicative d = new KApplicative(new PEncapD(id));
            KPair p3 = new KPair(d, new KNil());
            KPair p2 = new KPair(p, p3);
            KPair p1 = new KPair(e, p2);
            return p1;
        }
    }
}

