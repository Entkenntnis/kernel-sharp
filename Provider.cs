using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Kernel
{


    public class TokenProvider
    {
        private static List<TokenDefinition> cache = null;
        public static List<TokenDefinition> getDefinitions()
        {
            if (cache != null)
                return cache;
            List<TokenDefinition> definitions = new List<TokenDefinition>();
            definitions.Add(new TokenDefinition(new Regex(@"[\s]+"), "whitespace", 100, true));
            definitions.Add(new TokenDefinition(new Regex(@"\;[^\n]*(?:\n|$)"), "comment", 90, true));
            definitions.Add(new TokenDefinition(new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*"""), "string", 80)); // user
            definitions.Add(new TokenDefinition(new Regex(@"\(\)"), "nil", 70));
            definitions.Add(new TokenDefinition(new Regex(@"\("), "openpa", 60));
            definitions.Add(new TokenDefinition(new Regex(@"\)"), "clospa", 50));
            definitions.Add(new TokenDefinition(new Regex(@"\.(?=[\s\(])"), "dot", 40));
            definitions.Add(new TokenDefinition(new Regex(@"[^\s\(\)\;]+"), "text", 30));
            cache = definitions;
            return definitions;
        }
    }

    public class TokenHandlerProvider
    {
        private static Dictionary<string,Func<Token, KObject>> cache = null;
        public static Dictionary<string,Func<Token, KObject>> getHandler()
        {
            if (cache != null)
                return cache;
            Dictionary<string,Func<Token, KObject>> handler = new Dictionary<string,Func<Token, KObject>>();
            handler.Add("string", (Token x) => { // user
                string sub = x.Value.Substring(1, x.Value.Length - 2);
                sub = sub.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\r", "\r");
                return new KString(sub);
            });
            handler.Add("nil", (Token x) => {
                return new KNil();
            });
            cache = handler;
            return handler;
        }
    }

    public class TextHandlerProvider
    {
        private static List<TextHandler> cache = null;
        public static List<TextHandler> getHandler()
        {
            if (cache != null)
                return cache;
            List<TextHandler> handler = new List<TextHandler>();
            handler.Add(new TextHandler(100, (Token x) => {
                return x.Value == "#t"?new KBoolean(true):null;
            }));
            handler.Add(new TextHandler(95, (Token x) => {
                return x.Value == "#f"?new KBoolean(false):null;
            }));
            handler.Add(new TextHandler(90, (Token x) => {
                return x.Value == "#inert"?new KInert():null;
            }));
            handler.Add(new TextHandler(85, (Token x) => {
                return x.Value == "#ignore"?new KIgnore():null;
            }));
            handler.Add(new TextHandler(80, (Token x) => {
                return x.Value == "#empty-env"?new KEnvironment():null;
            }));
            handler.Add(new TextHandler(75, (Token x) => { // user
                return x.Value == "#e-infinity"?new KDouble(Double.NegativeInfinity):null;
            }));
            handler.Add(new TextHandler(70, (Token x) => { // user
                return x.Value == "#e+infinity"?new KDouble(Double.PositiveInfinity):null;
            }));
            handler.Add(new TextHandler(65, (Token x) => { // user
                long l;
                if (long.TryParse(x.Value, out l))
                        return new KInteger(l);
                else
                    return null;
            }));
            handler.Add(new TextHandler(60, (Token x) => { // user
                double d;
                if (!x.Value.Contains("Infinity") &&  double.TryParse(x.Value,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                    return new KDouble(d);
                return false;
            }));
            handler.Add(new TextHandler(0, (Token x) => {
                return new KSymbol(x.Value);
            }));
            cache = handler;
            return handler;
        }
    }
}

