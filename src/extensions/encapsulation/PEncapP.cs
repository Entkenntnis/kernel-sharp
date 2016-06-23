using System;

namespace Kernel
{
    public class PEncapP : POperative
    {
        private int id;
        public PEncapP(int id)
        {
            this.id = id;
        }

        public override string getName()
        {
            return "<#internal:encapP>";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            int len = KPair.Length(args);
            if (len == -1)
                throw new RuntimeException("parameter is not a list");
            else
            {
                bool result = true;
                KPair.Foreach(x =>
                    {
                        if (!(x is KEncapsulation) || (x as KEncapsulation).Id != id)
                            result = false;
                    }, args);
                return result;
            }
        }
    }
}

