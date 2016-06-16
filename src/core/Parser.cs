using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kernel
{

    public class Parser
    {
        private static bool initialized = false;

        private static List<TokenDefinition> definitions;
        private static List<TextHandler> textHandlers;
        private static Dictionary<string,Func<Token, KObject>> handlers;

        private static void init()
        {
            if (initialized)
                return;
            definitions = new List<TokenDefinition>();
            definitions.Add(new TokenDefinition(new Regex(@"[\s]+"), "whitespace", 100, true));
            definitions.Add(new TokenDefinition(new Regex(@"\;[^\n]*(?:\n|$)"), "comment", 90, true));
            definitions.Add(new TokenDefinition(new Regex(@"\(\)"), "nil", 70));
            definitions.Add(new TokenDefinition(new Regex(@"\("), "openpa", 60));
            definitions.Add(new TokenDefinition(new Regex(@"\)"), "clospa", 50));
            definitions.Add(new TokenDefinition(new Regex(@"\.(?=[\s\(])"), "dot", 40));
            definitions.Add(new TokenDefinition(new Regex(@"[^\s\(\)\;]+"), "text", 30));

            handlers = new Dictionary<string,Func<Token, KObject>>();

            handlers.Add("nil", (Token x) => {
                return new KNil();
            });

            textHandlers = new List<TextHandler>();
            textHandlers.Add(new TextHandler(100, (Token x) => {
                return x.Value == "#t"?new KBoolean(true):null;
            }));
            textHandlers.Add(new TextHandler(95, (Token x) => {
                return x.Value == "#f"?new KBoolean(false):null;
            }));
            textHandlers.Add(new TextHandler(90, (Token x) => {
                return x.Value == "#inert"?new KInert():null;
            }));
            textHandlers.Add(new TextHandler(85, (Token x) => {
                return x.Value == "#ignore"?new KIgnore():null;
            }));
            textHandlers.Add(new TextHandler(80, (Token x) => {
                return x.Value == "#empty-env"?new KEnvironment():null;
            }));
            textHandlers.Add(new TextHandler(0, (Token x) => {
                return new KSymbol(x.Value);
            }));

            initialized = true;
            update();
        }

        private static void update()
        {
            definitions.Sort((a, b) => {
                return b.Precedence - a.Precedence;
            });
            textHandlers.Sort((a, b) => {
                return b.Precedance - a.Precedance;
            });
        }

        public static void ExtendDefinition(TokenDefinition d)
        {
            init();
            definitions.Add(d);
            update();
        }

        public static void ExtendTextHandler(TextHandler h)
        {
            init();
            textHandlers.Add(h);
            update();
        }

        public static void ExtendHandler(string key, Func<Token, KObject> f)
        {
            init();
            handlers.Add(key, f);
        }



        public static List<Token> Lex(string datum)
        {
            init();
            int curIndex = 0;
            List<Token> tokens = new List<Token>();

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
                if (handlers.ContainsKey(cur.Label)) {
                    return handlers[cur.Label](cur);
                } else if ("text" == cur.Label) {
                    foreach (TextHandler f in textHandlers) {
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
            init();
            KObject ret = read(new TokenStream(Lex(tokens))) as KObject;
            if (null == ret)
                throw new ParseException("Empty token!");
            return ret;
        }

        public static List<KObject> ParseAll(string tokens)
        {
            init();
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

