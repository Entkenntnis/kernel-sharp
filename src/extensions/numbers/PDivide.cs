using System;

namespace Kernel
{
    public class PDivide : POperative
    {
        public PDivide()
        {
        }

        public override string getName()
        {
            return "/";
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
                    return new KFraction((a as KFraction).Denominator, (a as KFraction).Numerator);
                else {
                    return new KDouble(1.0 / (a as KDouble).Value);
                }
            } else {
                KObject quot = NumbersModule.Check(First(args));
                KObject head = (args as KPair).Cdr;
                while (head is KPair) {
                    KObject nextNumber = NumbersModule.Check(First(head));
                    if (quot is KFraction && nextNumber is KFraction) {
                        quot = (quot as KFraction).Divide(nextNumber as KFraction);
                    } else {
                        KDouble a = NumbersModule.ToInexact(quot);
                        KDouble b = NumbersModule.ToInexact(nextNumber);
                        quot = new KDouble(a.Value / b.Value);
                    }
                    head = (head as KPair).Cdr;
                }
                return quot;
            }
        }
    }
}

