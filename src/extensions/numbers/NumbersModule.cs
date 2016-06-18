using System;
using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Kernel
{
    public class NumbersModule : Module
    {
        public NumbersModule()
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
                        return new KFraction(i, BigInteger.One);
                else if (Regex.IsMatch(x.Value, @"-?[\d]+/[\d]+")) {
                    string[] parts = x.Value.Split(new char[]{'/'});
                    return new KFraction(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[1]));
                } else {
                    return null;
                }
                    
            }));
            Parser.ExtendTextHandler(new TextHandler(60, (Token x) => { // user
                double d;
                if (!x.Value.Contains("Infinity") && 
                        double.TryParse(x.Value,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                    return new KDouble(d);
                return null;
            }));
            Interpreter.AddOp(new PExactp());
            Interpreter.AddOp(new PInexactp());
            Interpreter.AddOp(new PExact2Inexact());
            Interpreter.AddOp(new PInexact2Exact());
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

