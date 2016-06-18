using System;
using System.Globalization;
using System.Numerics;

namespace Kernel
{
    public class NumberModule : Module
    {
        public NumberModule()
        {
        }

        public override void Init()
        {
            Parser.ExtendTextHandler(new TextHandler(75, (Token x) => { // user
                return x.Value == "#e-infinity"?new KDouble(Double.NegativeInfinity):null;
            }));
            Parser.ExtendTextHandler(new TextHandler(70, (Token x) => { // user
                return x.Value == "#e+infinity"?new KDouble(Double.PositiveInfinity):null;
            }));
            Parser.ExtendTextHandler(new TextHandler(65, (Token x) => { // user
                BigInteger i;
                if (BigInteger.TryParse(x.Value, out i))
                        return new KInteger(i);
                else
                    return null;
            }));
            Parser.ExtendTextHandler(new TextHandler(60, (Token x) => { // user
                double d;
                if (!x.Value.Contains("Infinity") && 
                        double.TryParse(x.Value,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                    return new KDouble(d);
                return null;
            }));
            Interpreter.ExtendGroundEnv("integer?", new KApplicative(new PIntegerp()));
            Interpreter.ExtendGroundEnv("double?", new KApplicative(new PDoublep()));
            Interpreter.ExtendGroundEnv("integer->double", new KApplicative(new PInteger2Double()));
            Interpreter.ExtendGroundEnv("double->integer", new KApplicative(new PDouble2Integer()));
            Interpreter.ExtendGroundEnv("integer-add", new KApplicative(new PIntegerAdd()));
            Interpreter.ExtendGroundEnv("integer-subtract", new KApplicative(new PIntegerSubtract()));
            Interpreter.ExtendGroundEnv("integer-multiply", new KApplicative(new PIntegerMultiply()));
            Interpreter.ExtendGroundEnv("integer-divide", new KApplicative(new PIntegerDivide()));
            Interpreter.ExtendGroundEnv("integer-less?", new KApplicative(new PIntegerLessp()));
            Interpreter.ExtendGroundEnv("double-add", new KApplicative(new PDoubleAdd()));
            Interpreter.ExtendGroundEnv("double-subtract", new KApplicative(new PDoubleSubtract()));
            Interpreter.ExtendGroundEnv("double-multiply", new KApplicative(new PDoubleMultiply()));
            Interpreter.ExtendGroundEnv("double-divide", new KApplicative(new PDoubleDivide()));
            Interpreter.ExtendGroundEnv("double-less?", new KApplicative(new PDoubleLessp()));
            Interpreter.ExtendGroundEnv("double-round", new KApplicative(new PDoubleRound()));
            Interpreter.LoadLibrary(new NumbersLibrary());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString()};
        }
    }
}

