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
                throw new ParseException("fraction can not have zero as denominator");
            }
            if (Denominator < 0) {
                Numerator *= -1;
                Denominator *= -1;
            }
            Reduce();
        }
        public void Reduce()
        {
            BigInteger gcd = BigInteger.GreatestCommonDivisor(Numerator, Denominator);
            Numerator /= gcd;
            Denominator /= gcd;
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
    }
}

