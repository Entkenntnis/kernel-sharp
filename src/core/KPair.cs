using System;
using System.Text;
using System.Collections.Generic;

namespace Kernel
{

    public class KPair : KObject
    {
        private static KObject copyEs(KObject x, bool mutable, Dictionary<KObject, KObject> visited)
        {
            if (!(x is KPair))
                return x;
            KPair n = new KPair(null, null, true);
            if (visited.ContainsKey(x))
                return visited[x];
            else
                visited.Add(x, n);
            KPair p = x as KPair;
            KObject car = p.Car is KPair ? copyEs(p.Car, mutable, visited) : p.Car;
            KObject cdr = p.Cdr is KPair ? copyEs(p.Cdr, mutable, visited) : p.Cdr;
            n.SetCar(car);
            n.SetCdr(cdr);
            if (!mutable)
                n.MakeImmutable();
            return n;
        }

        public static KObject CopyEs(KObject x)
        {
            return copyEs(x, true, new Dictionary<KObject, KObject>());
        }

        public static KObject CopyEsImmutable(KObject x)
        {
            return copyEs(x, false, new Dictionary<KObject, KObject>());
        }
        // move this to the evaluator! it is only used there
        public static KObject Map(Func<KObject, KObject> f, KObject lst)
        {
            KNil nil = new KNil();
            KPair newlst = null;
            KPair head = newlst;
            List<KObject> visited = new List<KObject>();
            while (true) {
                if (lst is KNil) {
                    if (null == head)
                        return nil;
                    else
                        return head;
                } else if (lst is KPair && visited.Contains(lst)) {
                    newlst.SetCdr(lst);
                    return newlst;
                } else if (lst is KPair) {
                    KPair orig = lst as KPair;
                    visited.Add(orig);
                    if (null == newlst)
                        head = newlst = new KPair(f(orig.Car), nil, true);
                    else {
                        KPair newElement = new KPair(f(orig.Car), nil, true);
                        newlst.SetCdr(newElement);
                        newlst = newElement;
                    }
                    lst = orig.Cdr;
                } else
                    throw new RuntimeException("Improper list passed to map");
            }                
        }

        public static void Foreach(Action<KObject> f, KObject lst)
        {
            List<KObject> visited = new List<KObject>();
            while (!(lst is KNil)) {
                if (lst is KPair) {
                    KPair cur = lst as KPair;
                    if (visited.Contains(cur))
                        return;
                    else
                        visited.Add(cur);
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
            List<KObject> visited = new List<KObject>();
            int length = 0;
            if (lst is KPair) {
                KPair cur = lst as KPair;
                visited.Add(cur);
                length++;
                while (true) {
                    if (cur.Cdr is KNil)
                        return length;
                    else if (cur.Cdr is KPair) {
                        cur = cur.Cdr as KPair;
                        if (visited.Contains(cur))
                            return -1;
                        else
                            visited.Add(cur);
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

        public bool Mutable {
            get;
            private set;
        }

        public KPair(KObject car, KObject cdr, bool mut)
        {
            Car = car;
            Cdr = cdr;
            Mutable = mut;
        }

        public void SetCar(KObject car)
        {
            if (Mutable)
                Car = car;
            else
                throw new Exception("Internal: Attempt to mutate immutable list!");
        }

        public void SetCdr(KObject cdr)
        {
            if (Mutable)
                Cdr = cdr;
            else
                throw new Exception("Internal: Attempt to mutate immutable list!");
        }

        public void MakeImmutable()
        {
            this.Mutable = false;
        }

        public override string Print(bool quoteStrings)
        {
            return Print(quoteStrings, new List<KObject>());
        }

        public override string Print(bool quoteStrings, List<KObject> visited)
        {
            if (visited.Contains(this))
                return "#sref";
            visited.Add(this);
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            KPair cur = this;
            while (true) {
                var subvis = new List<KObject>();
                subvis.AddRange(visited);
                sb.Append(cur.Car.Print(quoteStrings, subvis));
                if (cur.Cdr is KNil) {
                    sb.Append(")");
                    break;
                } else {
                    if (cur.Cdr is KPair) {
                        cur = (KPair)cur.Cdr;
                        sb.Append(" ");
                    } else {
                        sb.Append(" . ");
                        sb.Append(cur.Cdr.Print(quoteStrings, visited));
                        sb.Append(")");
                        break;
                    }
                }
                if (visited.Contains(cur)) {
                    sb.Append(". #sref)");
                    return sb.ToString();
                } else
                    visited.Add(cur);
            }
            return sb.ToString();
        }

        public override bool CompareTo(KObject other)
        {
            return (other is KPair) && other == this;
        }
    }
}

