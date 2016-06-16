using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kernel
{

    public class Parser
    {
        public static List<Token> Lex(string datum)
        {
            int curIndex = 0;
            List<Token> tokens = new List<Token>();
            List<TokenDefinition> definitions = TokenProvider.getDefinitions();
            definitions.Sort((a, b) => {
                return b.Precedence - a.Precedence;
            });
            while (curIndex < datum.Length) {
                bool success = false;
                foreach (var rule in definitions) {
                    Match match = rule.Rule.Match(datum, curIndex);
                    if (match.Success && match.Index == curIndex) {
                        success = true;
                        if (!rule.Ignore) {
                            tokens.Add(new Token(match.Value, rule.Label));
                        }
                        curIndex += match.Length;
                        break;
                    }
                }
                if (!success) {
                    throw new ParseException("Unrecognised pattern");
                }
            }
            return tokens;
        }

        private static object read(TokenStream s)
        {
            Token cur = s.Next();
            if (null == cur)
                return null;
            if ("clospa" == cur.Label)
                return ')';
            if ("dot" == cur.Label)
                return '.';
            if ("openpa" != cur.Label) {
                var handler = TokenHandlerProvider.getHandler();
                if (handler.ContainsKey(cur.Label)) {
                    return handler[cur.Label](cur);
                } else if ("text" == cur.Label) {
                    var th = TextHandlerProvider.getHandler();
                    th.Sort((a, b) => {
                        return b.Precedance - a.Precedance;
                    });
                    foreach (TextHandler f in th) {
                        object ret = f.Handler(cur);
                        if (ret as KObject != null)
                            return ret;
                    }
                    throw new ParseException("unhandled text");
                } else {
                    throw new ParseException("unknown token");
                }
            } else {
                // list stuff
                List<object> content = new List<object>();
                object v = read(s);
                bool dotted = false;
                while (true) {
                    if ((v is char) && ((char)v) == '.') {
                        v = read(s);
                        if (((char)read(s)) != ')') {
                            throw new ParseException("Illegal use of dot");
                        }
                        if (v is KObject)
                            content.Add(v);
                        else
                            throw new ParseException("Illegal use of dot");
                        dotted = true;
                        break;
                    } else if ((v is char) && ((char)v) == ')') {
                        break;
                    } else if (v is KObject) {
                        content.Add(v);
                        v = read(s);
                    } else {
                        throw new ParseException("Parse error");
                    }
                }
                int index = content.Count - 1;
                KPair tail;
                if (dotted) {
                    tail = new KPair(content[index - 1] as KObject, content[index] as KObject, false);
                    index -= 2;
                } else {
                    tail = new KPair(content[index--] as KObject, new KNil(), false);
                }
                for (int i = index; i >= 0; i--) {
                    tail = new KPair(content[i] as KObject, tail, false);
                }
                return tail;
            }
        }

        public static KObject Parse(string tokens)
        {
            KObject ret = read(new TokenStream(Lex(tokens))) as KObject;
            if (null == ret)
                throw new ParseException("Empty token!");
            return ret;
        }

        public static List<KObject> ParseAll(string tokens)
        {
            var output = new List<KObject>();
            TokenStream s = new TokenStream(Lex(tokens));
            KObject cur = read(s) as KObject;
            while (cur != null) {
                output.Add(cur);
                cur = read(s) as KObject;
            }
            return output;
        }
         
    }
}

