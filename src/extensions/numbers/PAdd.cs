using System;
using System.Numerics;

namespace Kernel
{
    public class PAdd : POperative
    {
        public PAdd()
        {
        }

        public override string getName()
        {
            return "+";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            int length = KPair.Length(args);
            if (length == -1)
                throw new RuntimeException("improper list");
            if (length == 0) {
                return new KFraction(BigInteger.Zero, BigInteger.One);
            } else {
                // we have a list of at least one elements
                // we must fold it to something useful
                KObject sum = NumbersModule.Check(First(args));
                KObject head = (args as KPair).Cdr;
                while (head is KPair) {
                    KObject nextNumber = NumbersModule.Check(First(head));
                    if (sum is KFraction && nextNumber is KFraction) {
                        sum = (sum as KFraction).Add(nextNumber as KFraction);
                    } else {
                        KDouble a = NumbersModule.ToInexact(sum);
                        KDouble b = NumbersModule.ToInexact(nextNumber);
                        sum = new KDouble(a.Value + b.Value);
                    }
                    head = (head as KPair).Cdr;
                }
                return sum;
            }
        }
    }
}

