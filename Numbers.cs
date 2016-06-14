using System;

namespace Kernel
{
    public class KNumber
    {
        public static bool IsNumber(KObject a)
        {
            return (a is KInteger || a is KDouble);
        }

        public static Double GetDouble(KObject a)
        {
            if (a is KInteger)
                return (double)(a as KInteger).Value;
            else if (a is KDouble)
                return (a as KDouble).Value;
            else
                return 0;
        }

        public static KObject DoMath(KObject a, KObject b, Func<Double, Double , Double> dop, Func<long, long, long> lop)
        {
            if (!(KNumber.IsNumber(a) && KNumber.IsNumber(b)))
                return null;
            if (a is KDouble || b is KDouble)
            {
                return new KDouble(dop(KNumber.GetDouble(a),KNumber.GetDouble(b)));
            }
            else
                return new KInteger(lop((a as KInteger).Value,(b as KInteger).Value));
        }

    }

    public class PAdd : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "+");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            var result = KNumber.DoMath(a, b, (x, y) => x + y, (x, y) => x + y);
            if (result == null)
                return CPS.Error("+: wrong types", cont);
            return Return(result, cont);
        }
    }

    public class PSub : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "-");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            var result = KNumber.DoMath(a, b, (x, y) => x - y, (x, y) => x - y);
            if (result == null)
                return CPS.Error("-: wrong types", cont);
            return Return(result, cont);
        }
    }

    public class PMult : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "*");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            var result = KNumber.DoMath(a, b, (x, y) => x * y, (x, y) => x * y);
            if (result == null)
                return CPS.Error("*: wrong types", cont);
            return Return(result, cont);
        }
    }

    public class PDiv : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "-");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            if (KNumber.IsNumber(b) && KNumber.GetDouble(b) == 0.0)
                return CPS.Error("Division by zero", cont);
            var result = KNumber.DoMath(a, b, (x, y) => x / y, (x, y) => x / y);
            if (result == null)
                return CPS.Error("/: wrong types", cont);
            return Return(result, cont);
        }
    }

    public class PNumber : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 1, "number?");
            if (res != null)
                return CPS.Error(res, cont);
            return ReturnBool(First(args) is KDouble || First(args) is KInteger, cont);
        }
    }

    public class PInteger : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "integer?", typeof(KInteger), cont);
        }
    }

    public class PInexact : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Predicate(args, "inexcat?", typeof(KDouble), cont);
        }
    }

    public class PNEq : KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "=?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            if (!(KNumber.IsNumber(a)&&KNumber.IsNumber(b)))
                return CPS.Error("wrong types", cont);
            if (a is KDouble || b is KDouble)
            {
                return ReturnBool(KNumber.GetDouble(a) == KNumber.GetDouble(b), cont);
            }
            else
                return ReturnBool((a as KInteger).Value == (b as KInteger).Value, cont);
        }
    }

    public class PNless: KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, "<?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            if (!(KNumber.IsNumber(a)&&KNumber.IsNumber(b)))
                return CPS.Error("wrong types", cont);
            if (a is KDouble || b is KDouble)
            {
                return ReturnBool(KNumber.GetDouble(a) < KNumber.GetDouble(b), cont);
            }
            else
                return ReturnBool((a as KInteger).Value < (b as KInteger).Value, cont);
        }
    }

    public class PNgreater: KOperative
    {
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            var res = CheckParameter(args, 2, ">?");
            if (res != null)
                return CPS.Error(res, cont);
            KObject a = First(args), b = Second(args);
            if (!(KNumber.IsNumber(a)&&KNumber.IsNumber(b)))
                return CPS.Error("wrong types", cont);
            if (a is KDouble || b is KDouble)
            {
                return ReturnBool(KNumber.GetDouble(a) > KNumber.GetDouble(b), cont);
            }
            else
                return ReturnBool((a as KInteger).Value > (b as KInteger).Value, cont);
        }
    }
}

