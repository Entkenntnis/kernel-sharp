using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace Kernel
{



    // object definitions



    public class KString : KObject
    {
        public string Value
        {
            get;
            private set;
        }
        public KString(string val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            if (quoteStrings)
                return ToLiteral(Value);
            else
                return Value;
        }
        private static string ToLiteral(string input)
        {
            return "\"" + input.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("\t", "\\t").Replace("\r", "\\r") + "\"";
        }
    }

    public class KInteger : KObject
    {
        public long Value
        {
            get;
            private set;
        }
        public KInteger(long val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            return Value.ToString();
        }
    }

    public class KDouble : KObject
    {
        public double Value
        {
            get;
            private set;
        }

        public KDouble(double val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

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
    }

    public struct KGuard
    {
        public KContinuation Selector;
        public KApplicative Interceptor;
    }

    public class KContinuation : KObject
    {
        public Continuation<KObject> Value
        {
            get;
            private set;
        }
        private Continuation<KObject> fixStructure(Continuation<KObject> c)
        {
            if (c.Parent == null)
                return c; // base continuation without state
            else
            {
                var thatsnew = new Continuation<KObject>(c.NextStep, fixStructure(c.Parent), c.Context);
                thatsnew.isError = c.isError;
                if (c._Placeholder != null)
                {
                    var remaing = new LinkedList<KObject>(c._RemainingObjs);
                    var pars = new LinkedList<KObject>(c._Pairs);
                    thatsnew._Pairs = pars;
                    thatsnew._RemainingObjs = remaing;
                    thatsnew._Placeholder = c._Placeholder;
                }
                return thatsnew;
            }
        }
        public KContinuation(Continuation<KObject> cont)
        {
            Value = fixStructure(cont);
        }
        public override string Print(bool quoteStrings)
        {
            return "#<continuation>";
        }
    }
}