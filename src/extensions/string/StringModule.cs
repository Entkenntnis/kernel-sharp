using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Kernel
{
    public class StringModule : Module
    {
        public StringModule()
        {
        }

        public override void Init()
        {
            Parser.ExtendDefinition(
                new TokenDefinition(new Regex(@"""[^""\\]*(?:\\.[^""\\]*)*"""), "string", 80));
            Parser.ExtendHandler("string", (Token x) => { // user
                string sub = x.Value.Substring(1, x.Value.Length - 2);
                sub = sub.Replace("\\n", "\n").Replace("\\t", "\t").Replace("\\\"", "\"").Replace("\\r", "\r");
                return new KString(sub);
            });
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

