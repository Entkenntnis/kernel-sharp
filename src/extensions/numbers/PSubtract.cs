using System;
using System.Numerics;

namespace Kernel
{
    public class PSubtract : POperative
    {
        public PSubtract()
        {
        }

        public override string getName()
        {
            return "-";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            int length = KPair.Length(args);
            if (length == -1)
                throw new RuntimeException("improper list");
            if (length == 0) {
                throw new RuntimeException("at least one arg expected");
            } else if (length == 1) {
                KObject a = NumbersModule.Check(First(args));
                if (a is KFraction)
                    return new KFraction((a as KFraction).Numerator * -1, (a as KFraction).Denominator);
                else {
                    return new KDouble((a as KDouble).Value * -1);
                }
            } else {
                KObject dif = NumbersModule.Check(First(args));
                KObject head = (args as KPair).Cdr;
                while (head is KPair) {
                    KObject nextNumber = NumbersModule.Check(First(head));
                    if (dif is KFraction && nextNumber is KFraction) {
                        dif = (dif as KFraction).Subtract(nextNumber as KFraction);
                    } else {
                        KDouble a = NumbersModule.ToInexact(dif);
                        KDouble b = NumbersModule.ToInexact(nextNumber);
                        dif = new KDouble(a.Value - b.Value);
                    }
                    head = (head as KPair).Cdr;
                }
                return dif;
            }
        }
    }
}

