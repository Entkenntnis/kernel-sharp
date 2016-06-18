using System;

namespace Kernel
{
    public abstract class POperative : KOperative
    {
        public virtual RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            object result = null;
            try {
                result = Do(args, env, cont);
            } catch (RuntimeException e) {
                return CPS.Error<KObject>(getName() + ": " + e.Message, cont);
            }
            if (result is KObject)
                return CPS.Return(result as KObject, cont);
            else if (result is bool)
                return CPS.Return(new KBoolean((bool)result), cont);
            else if (result is RecursionResult<KObject>)
                return result as RecursionResult<KObject>;
            else
                return CPS.Return(new KInert(), cont);
        } 

        public abstract string getName();

        public virtual object Do(KObject args, KEnvironment env, Continuation<KObject> cont) {
            return false;
        }

        protected void CPara(KObject p, int len)
        {
            int actLen = KPair.Length(p);
            if (actLen != len)
            {
                throw new RuntimeException("mismatching number of ops, expected " + len + ", got " + actLen);
            }
        }
        protected void Check(object obj, string message = "wrong type")
        {
            if (null == obj)
                throw new RuntimeException(message);
        }
        protected KObject First(KObject p)
        {
            return ((KPair)p).Car;
        }
        protected KObject Second(KObject p)
        {
            return ((KPair)((KPair)p).Cdr).Car;
        }
        protected KObject Third(KObject p)
        {
            return (((KPair)((KPair)p).Cdr).Cdr as KPair).Car;
        }
    }
}

