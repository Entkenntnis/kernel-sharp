using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kernel
{

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message){ }
    }

    public class TokenDefinition
    {
        public Regex Rule { get; private set; }
        public string Label { get; private set; }
        public bool Ignore { get; private set; }
        public int Precedence { get; private set; }
        public TokenDefinition(Regex rule, string label, int prec = 0, bool ignore = false)
        {
            Rule = rule;
            Label = label;
            Ignore = ignore;
            Precedence = prec;
        }
    }

    public class Token
    {
        public string Value { get; private set; }
        public string Label { get; private set; }
        public Token(string value, string label)
        {
            Value = value;
            Label = label;
        }
        public override string ToString()
        {
            return string.Format("[Token: Value={0}, Label={1}]", Value, Label);
        }
    }

    public class TokenProvider
    {
        public static List<TokenDefinition> getDefinitions()
        {
            List<TokenDefinition> definitions = new List<TokenDefinition>();
            definitions.Add(new TokenDefinition(new Regex(@"[\s]+"), "whitespace", 100, true));
            definitions.Add(new TokenDefinition(new Regex(@"\;[^\n]*(?:\n|$)"), "comment", 90, true));
            definitions.Add(new TokenDefinition(new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*"""), "string", 80));
            definitions.Add(new TokenDefinition(new Regex(@"\(\)"), "nil", 70));
            definitions.Add(new TokenDefinition(new Regex(@"\("), "openpa", 60));
            definitions.Add(new TokenDefinition(new Regex(@"\)"), "clospa", 50));
            definitions.Add(new TokenDefinition(new Regex(@"\.(?=[\s\(])"), "dot", 40));
            definitions.Add(new TokenDefinition(new Regex(@"[^\s\(\)\;]+"), "text", 30));
            return definitions;
        }
    }

    public class TokenStream
    {
        private int index;
        private List<Token> data;
        public TokenStream(List<Token> input)
        {
            data = new List<Token>(input);
            index = 0;
        }
        public Token Next()
        {
            if (index < data.Count)
            {
                return data[index++];
            }
            else
            {
                return null;
            }
        }
    }

    public class Parser
    {
        public static List<Token> Lex(string datum)
        {
            int curIndex = 0;
            List<Token> tokens = new List<Token>();
            List<TokenDefinition> definitions = TokenProvider.getDefinitions();
            definitions.Sort((a,b)=>{return b.Precedence - a.Precedence;});
            while (curIndex < datum.Length)
            {
                bool success = false;
                foreach (var rule in definitions)
                {
                    Match match = rule.Rule.Match(datum, curIndex);
                    if (match.Success && match.Index == curIndex)
                    {
                        success = true;
                        if (!rule.Ignore)
                        {
                            tokens.Add(new Token(match.Value, rule.Label));
                        }
                        curIndex += match.Length;
                        break;
                    }
                }
                if (!success)
                {
                    throw new ParseException("Unrecognised pattern");
                }
            }
            return tokens;
        }

        public static object Read(TokenStream s)
        {
            Token cur = s.Next();
            if (null == cur)
                return null;
            if ("clospa" == cur.Label)
                return ')';
            if ("dot" == cur.Label)
                return '.';
            if ("openpa" != cur.Label)
            {
                // some more cases here
                if ("nil" == cur.Label)
                {
                    return new KNil();
                }
                if ("string" == cur.Label)
                {
                    string sub = cur.Value.Substring(1, cur.Value.Length - 2);
                    sub = sub.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\r", "\r");
                    return new KString(sub);
                }
                else if ("text" == cur.Label)
                {
                    if (cur.Value == "#t")
                    {
                        return new KBoolean(true);
                    }
                    if (cur.Value == "#f")
                    {
                        return new KBoolean(false);
                    }
                    if (cur.Value == "#empty-env")
                    {
                        return new KEnvironment();
                    }
                    if (cur.Value == "#ignore")
                    {
                        return new KIgnore();
                    }
                    if (cur.Value == "#inert")
                    {
                        return new KInert();
                    }
                    if (cur.Value == "#e-infinity")
                        return new KDouble(Double.NegativeInfinity);
                    if (cur.Value == "#e+infinity")
                        return new KDouble(Double.PositiveInfinity);
                    long x;
                    if (long.TryParse(cur.Value, out x))
                        return new KInteger(x);

                    double d;
                    if (!cur.Value.Contains("Infinity") &&  double.TryParse(cur.Value,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                        return new KDouble(d);

                    return new KSymbol(cur.Value);
                }
                else
                {
                    throw new ParseException("unknown token");
                }
            }
            else
            {
                // list stuff
                List<object> content = new List<object>();
                object v = Read(s);
                bool dotted = false;
                while (true)
                {
                    if ((v is char) && ((char)v) == '.')
                    {
                        v = Read(s);
                        if (((char)Read(s)) != ')')
                        {
                            throw new ParseException("Illegal use of dot");
                        }
                        if (v is KObject)
                            content.Add(v);
                        else
                            throw new ParseException("Illegal use of dot");
                        dotted = true;
                        break;
                    }
                    else if ((v is char) && ((char)v) == ')')
                    {
                        break;
                    }
                    else if (v is KObject)
                    {
                        content.Add(v);
                        v = Read(s);
                    }
                    else
                    {
                        throw new ParseException("Parse error");
                    }
                }
                int index = content.Count - 1;
                KPair tail;
                if (dotted)
                {
                    tail = new KPair(content[index-1] as KObject,content[index] as KObject,false);
                    index -= 2;
                } else {
                    tail = new KPair(content[index--] as KObject, new KNil(), false);
                }
                for (int i = index; i >= 0; i--)
                {
                    tail = new KPair(content[i] as KObject, tail, false);
                }
                return tail;
            }
        }

        public static KObject Parse(string tokens)
        {
            KObject ret = Read(new TokenStream(Lex(tokens))) as KObject;
            if (null == ret)
                throw new ParseException("Empty token!");
            return ret;
        }

        public static List<KObject> ParseAll(string tokens)
        {
            var output = new List<KObject>();
            TokenStream s = new TokenStream(Lex(tokens));
            KObject cur = Read(s) as KObject;
            while (cur != null)
            {
                output.Add(cur);
                cur = Read(s) as KObject;
            }
            return output;
        }
         
    }
}

