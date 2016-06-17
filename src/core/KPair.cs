using System;
using System.Text;
using System.Collections.Generic;

namespace Kernel
{

    public class KPair : KObject
    {
        public static void Foreach(Action<KObject> f, KObject lst)
        {
            while (!(lst is KNil)) {
                if (lst is KPair) {
                    KPair cur = lst as KPair;
                    f(cur.Car);
                    lst = cur.Cdr;
                } else
                    throw new RuntimeException("Improper list passed to foreach");
            }
            return;
        }

        public static int Length(KObject lst)
        {
            if (lst is KNil)
                return 0;
            int length = 0;
            if (lst is KPair) {
                KPair cur = lst as KPair;
                length++;
                while (true) {
                    if (cur.Cdr is KNil)
                        return length;
                    else if (cur.Cdr is KPair) {
                        cur = cur.Cdr as KPair;
                        length++;
                    } else
                        return -1;
                }
            } else
                return -1;
        }

        public KObject Car {
            get;
            private set;
        }

        public KObject Cdr {
            get;
            private set;
        }

        public KPair(KObject car, KObject cdr)
        {
            Car = car;
            Cdr = cdr;
        }

        protected void SetCar(KObject car)
        {
            Car = car;
        }

        protected void SetCdr(KObject cdr)
        {
            Cdr = cdr;
        }

        public override string Print(bool quoteStrings)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            KPair cur = this;
            while (true) {
                sb.Append(cur.Car.Print(quoteStrings));
                if (cur.Cdr is KNil) {
                    sb.Append(")");
                    break;
                } else {
                    if (cur.Cdr is KPair) {
                        cur = (KPair)cur.Cdr;
                        sb.Append(" ");
                    } else {
                        sb.Append(" . ");
                        sb.Append(cur.Cdr.Print(quoteStrings));
                        sb.Append(")");
                        break;
                    }
                }
            }
            return sb.ToString();
        }

        public override bool CompareTo(KObject other)
        {
            return (other is KPair) && other == this;
        }
    }
}