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
                    try {
                    return new KFraction(BigInteger.Parse(parts[0]), BigInteger.Parse(parts[1]));
                    } catch (Exception)
                        {return null;}
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
            Interpreter.AddOp(new PAdd());
            Interpreter.AddOp(new PSubtract());
            Interpreter.AddOp(new PMultiply());
            Interpreter.AddOp(new PDivide());
            Interpreter.AddOp(new PLessp());
            Interpreter.AddOp(new PLessEqualp());
            Interpreter.AddOp(new PEqualp());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new OperatorsModule().ToString()};
        }

        public override string getLibrary()
        {
            return @"
($define! >?
    ($lambda args
        (apply <? (reverse args))))

($define! >=?
    ($lambda args
        (apply <=? (reverse args))))

            ";
        }

        public static KObject Check(KObject obj)
        {
            if (!(obj is KDouble) && !(obj is KFraction))
                throw new RuntimeException("expected number");
            return obj;
        }

        public static KDouble ToInexact(KObject obj)
        {
            if (obj is KDouble)
                return obj as KDouble;
            else
                return new KDouble((obj as KFraction).ToDouble());
        }
        public static KFraction ToExact(KObject obj)
        {
            if (obj is KFraction)
                return obj as KFraction;
            else {
                return KFraction.fromDouble((obj as KDouble).Value);
            }
        }
        public static bool Numberp(KObject args, Func<KFraction, KFraction, bool> cmp)
        {
            int length = KPair.Length(args);
            if (length < 2) {
                throw new RuntimeException("at least two arguments");
            } else {
                KFraction first = ToExact(Check((args as KPair).Car));
                KObject head = (args as KPair).Cdr;
                while (head is KPair) {
                    KFraction next = NumbersModule.ToExact(NumbersModule.Check((head as KPair).Car));
                    if (!cmp(first, next))
                        return false;

                    first = next;
                    head = (head as KPair).Cdr;
                }
                return true;
            }
        }
    }
}

