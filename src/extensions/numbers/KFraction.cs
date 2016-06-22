using System;
using System.Numerics;

namespace Kernel
{
    public class KFraction : KObject
    {
        public BigInteger Numerator {
            get;
            private set;
        }
        public BigInteger Denominator {
            get;
            private set;
        }
        public KFraction(BigInteger num, BigInteger denom)
        {
            Numerator = num;
            Denominator = denom;
            if (Denominator.Equals(BigInteger.Zero)) {
                throw new RuntimeException("fraction can not have zero as denominator");
            }
            if (Denominator < 0) {
                Numerator *= -1;
                Denominator *= -1;
            }
            Reduce();
        }
        public static KFraction fromDouble(double input)
        {
            if (Double.IsInfinity(input))
                throw new RuntimeException("can not make infinity exact");
            bool negativ = false;
            if (input < 0) {
                negativ = true;
                input *= -1;
            }
            BigInteger num = (BigInteger)Math.Floor(input);
            BigInteger denom = 1;
            double rem = input - Math.Floor(input);
            while (rem > 0) {
                num *= 2;
                denom *= 2;
                rem *= 2.0;
                num += (BigInteger)Math.Floor(rem);
                rem -= Math.Floor(rem);
            }
            if (negativ)
                num *= -1;
            return new KFraction(num, denom);
        }
        public KFraction Reduce()
        {
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
            Numerator /= gcd;
            Denominator /= gcd;
            return this;
        }

        public override string Print(bool quoteStrings)
        {
            if (Denominator.Equals(BigInteger.One))
                return Numerator.ToString("R");
            else
                return Numerator.ToString("R") + "/" + Denominator.ToString("R");
        }

        public override bool CompareTo(KObject other)
        {
            return other is KFraction && (other as KFraction).Denominator.Equals(Denominator)
                && (other as KFraction).Numerator.Equals(Numerator);
        }
        public double ToDouble()
        {
            double num = (double)Numerator;
            double denom = (double)Denominator;
            return num/denom;
        }
        public KFraction Add(KFraction b)
        {
            BigInteger cm = Denominator * b.Denominator;
            BigInteger newNum = Numerator * b.Denominator + b.Numerator * Denominator;
            return new KFraction(newNum, cm).Reduce();
        }
        public KFraction Subtract(KFraction b)
        {
            BigInteger cm = Denominator * b.Denominator;
            BigInteger newNum = Numerator * b.Denominator - b.Numerator * Denominator;
            return new KFraction(newNum, cm).Reduce();
        }
        public KFraction Multiply(KFraction b)
        {
            return new KFraction(Numerator*b.Numerator, Denominator*b.Denominator).Reduce();
        }
        public KFraction Divide(KFraction b)
        {
            return Multiply(new KFraction(b.Denominator, b.Numerator));
        }
        public bool LessThan(KFraction b)
        {
            return Numerator * b.Denominator < b.Numerator * Denominator;
        }
        public bool LessEqual(KFraction b)
        {
            return Numerator * b.Denominator <= b.Numerator * Denominator;
        }
    }
}

