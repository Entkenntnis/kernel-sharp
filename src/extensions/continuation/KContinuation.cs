using System;

namespace Kernel
{
    public class KContinuation : KObject
    {
        public Continuation<KObject> Value
        {
            get;
            private set;
        }
        public KContinuation(Continuation<KObject> cont)
        {
            Value = cont;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<continuation>";
        }
        public override bool CompareTo(KObject other)
        {
            return other is KContinuation && other == this;
        }
    }
}

