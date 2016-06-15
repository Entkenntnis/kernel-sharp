using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Kernel
{

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message){ }
    }

    public class Parser
    {
        private static string _data;
        private static int _dataIndex;

        public static KObject Parse(string tokens)
        {
            _data = tokens;
            _dataIndex = 0;
            KObject ret = buildExpr();
            if (null == ret)
                throw new ParseException("Empty token!");
            return ret;
        }

        public static List<KObject> ParseAll(string tokens)
        {
            _data = tokens;
            _dataIndex = 0;
            var output = new List<KObject>();
            KObject cur = buildExpr();
            while (cur != null)
            {
                output.Add(cur);
                cur = buildExpr();
            }
            return output;
        }

        private static KObject buildExpr()
        {
            return buildExpr(nextToken());
        }

        private static KObject buildExpr(object next)
        {
            if (next is KObject)
                return (KObject)next;
            else if (next is char)
            {
                // list handling
                char nextc = (char)next;
                if (nextc != '(')
                    throw new ParseException("Unbalanced paranthesis!");
                
                object secondnext = nextToken();
                if (secondnext == null)
                    return null;
                if (secondnext is char && ((char)secondnext) == ')')
                    return new KNil();

                KObject secondnextobj;
                if (secondnext is char && ((char)secondnext) == '(')
                    secondnextobj = buildExpr(secondnext);
                else
                    secondnextobj = secondnext as KObject;

                KPair listTail = new KPair(secondnextobj, null, true);
                KPair listHead = listTail;

                while (true)
                {
                    secondnext = nextToken();
                    if (secondnext == null)
                        throw new ParseException("Open Parens not closed!");
                    if (secondnext is char && (char)secondnext == ')')
                        break;
                    if (secondnext is char && (char)secondnext == '.')
                    {
                        object thirdnext = nextToken();
                        listTail.SetCdr(buildExpr(thirdnext));
                        object forthnext = nextToken();
                        if (forthnext is char && (char)forthnext == ')')
                            break;
                        else
                            throw new ParseException("Dotted list unbalanced!");
                    }
                    KPair newPair = new KPair(buildExpr(secondnext), null, true);
                    listTail.SetCdr(newPair);
                    listTail = newPair;
                }
                if (listTail.Cdr == null)
                    listTail.SetCdr(new KNil());
                
                return KPair.CopyEsImmutable(listHead);
            }
            else
                return null;
        }

        private static object nextChar()
        {
            if (_dataIndex == _data.Length)
                return null;
            char t = _data[_dataIndex];
            _dataIndex++;
            if (t != '\\')
                return t;
            else if (_dataIndex + 1 == _data.Length)
                return t;
            else
            {
                _dataIndex++;
                return _data.Substring(_dataIndex - 2, 2);
            }
        }

        private static char unescape(string x)
        {
            char c = x[1];
            if (c == 'n')
                return '\n';
            if (c == 't')
                return '\t';
            if (c == 'r')
                return '\r';
            if (c == '"')
                return '"';
            if (c == '\\')
                return '\\';
            else
                throw new ParseException("Unknown escape char: " + c.ToString());
        }

        private static object nextToken()
        {
            object thisT;

            // skip whitespace
            while (true)
            {
                thisT = nextChar();
                if (null == thisT)
                    return null;
                if (thisT is char && char.IsWhiteSpace((char)thisT))
                    continue;
                else if (thisT is char && (char)thisT == ';')
                {
                    while (thisT is char && (char)thisT != '\n')
                        thisT = nextChar();
                }
                else
                    break;
            }

            // parse string
            if (thisT is char && (char)thisT == '"')
            {
                StringBuilder sb = new StringBuilder();
                object nextC;
                while (true)
                {
                    nextC = nextChar();
                    if (null == nextC)
                        throw new ParseException("Unbalanced string!");
                    if (nextC is char && (char)nextC  == '"')
                        break;
                    if (nextC is string)
                        sb.Append(unescape((string)nextC));
                    else
                        sb.Append(((char)nextC).ToString());
                }
                return new KString(sb.ToString());
            }

            // unparsed
            if (thisT is char)
            {
                char c = (char)thisT;
                if (c == '(' || c == ')' || c == '.')
                    return c;
            }

            // get joined characters
            StringBuilder joined = new StringBuilder();
            object thatT = thisT;
            while (true)
            {
                if (null == thatT)
                    break;
                else if (thatT is char)
                {
                    char c = (char)thatT;
                    if (c == '(' || c == ')' || char.IsWhiteSpace(c) || c == ';')
                    {
                        _dataIndex--;
                        break;
                    }
                    else
                        joined.Append(((char)thatT).ToString());
                }
                else
                {
                    joined.Append(unescape((string)thatT));
                }
                thatT = nextChar();
            }
            string tok = joined.ToString().ToLower(CultureInfo.InvariantCulture);

            // check hashtag token
            if (tok.StartsWith("#"))
            {
                string tag = tok.Substring(1);
                if (tag.Equals("t"))
                    return new KBoolean(true);
                else if (tag.Equals("f"))
                    return new KBoolean(false);
                else if (tag.Equals("inert"))
                    return new KInert();
                else if (tag.Equals("ignore"))
                    return new KIgnore();
                else if (tag.Equals("empty-env"))
                    return new KEnvironment();
                else if (tag.Equals("e-infinity"))
                    return new KDouble(Double.NegativeInfinity);
                else if (tag.Equals("e+infinity"))
                    return new KDouble(Double.PositiveInfinity);
            }

            // try long number
            long x;
            if (long.TryParse(tok, out x))
                return new KInteger(x);

            // then try double
            double d;
            if (!tok.Contains("Infinity") &&  double.TryParse(tok,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                return new KDouble(d);

            // well, so it must be a symbol
            return new KSymbol(tok);
        }
    }
}

