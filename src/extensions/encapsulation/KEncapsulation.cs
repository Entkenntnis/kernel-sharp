using System;

namespace Kernel
{
    public class KEncapsulation : KObject
    {
        public int Id
        {
            get;
            private set;
        }
        public KObject Value
        {
            get;
            private set;
        }
        public KEncapsulation(int id, KObject value)
        {
            this.Id = id;
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<encapsulation:" + Id + ">";
        }
        public override bool CompareTo(KObject other)
        {
            return other is KEncapsulation && other == this;
        }
    }
}

