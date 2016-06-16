using System;
using System.Text.RegularExpressions;

namespace Kernel
{
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
}

