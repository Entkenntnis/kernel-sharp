using System;
using System.Numerics;

namespace Kernel
{
    public class PDouble2Integer : POperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = PHelper.CheckParameter(args, 1, "double->integer");
            if (null != res)
                return PHelper.Error(res, cont);
            KDouble d = PHelper.First(args) as KDouble;
            if (null == d)
                return PHelper.Error("double->integer: input not double", cont);
            if (Double.IsInfinity(d.Value))
                return PHelper.Error("double->integer: integer can not represent infinity", cont);
            return PHelper.Return(new KInteger((BigInteger)d.Value), cont);
        }
    }
}

