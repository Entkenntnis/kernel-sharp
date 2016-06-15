using System;
using System.Collections.Generic;

namespace Kernel
{
    public class KEnvironment : KObject
    {
        private static KEnvironment GroundEnv = null;
        public static KEnvironment GetGroundEnv()
        {
            if (null == GroundEnv)
            {
                GroundEnv = new KEnvironment(null);
                addp("boolean?", new PBoolean());
                addp("equal?", new PEqual());
                addp("eq?", new PEq());
                addp("equal?", new PEqual());
                addp("symbol?", new PSymbol());
                addp("inert?", new PInert());
                addp("pair?", new PPair());
                addp("null?", new PNull());
                addp("environment?", new PEnvironment());
                addp("ignore?", new PIgnore());
                addp("operative?", new POperative());
                addp("applicative?", new PApplicative());
                addp("cons", new PCons());
                addp("eval", new PEval());
                addp("make-environment", new PMakeEnvironment());
                addp("set-car!", new PSetCar());
                addp("set-cdr!", new PSetCdr());
                addp("copy-es-immutable", new PCopyEsImmutable());
                addp("copy-es", new PCopyEs());
                addp("wrap", new PWrap());
                addp("unwrap", new PUnwrap());
                GroundEnv.Bind("$if", new PIf());
                GroundEnv.Bind("$define!", new PDefine());
                GroundEnv.Bind("$vau", new PVau());
                addp("+", new PAdd());
                addp("-", new PSub());
                addp("*", new PMult());
                addp("/", new PDiv());
                addp("number?", new PNumber());
                addp("integer?", new PInteger());
                addp("=?", new PNEq());
                addp("<?", new PNless());
                addp(">?", new PNgreater());
                addp("inexact?", new PInexact());
                addp("write", new PWrite());
                addp("display", new PDisplay());
                addp("make-encapsulation-type", new PEncap());
                addp("continuation?", new PContinuation());
                addp("call/cc", new PCallCC());
                addp("continuation->applicative", new PContinuationToApplicative());
                addp("extend-continuation", new PExtendContinuatione());
                GroundEnv.Bind("root-continuation", new KContinuation(CPS.RootContinuation<KObject>()));
                var errorC = CPS.RootContinuation<KObject>();
                errorC.isError = true;
                GroundEnv.Bind("error-continuation", new KContinuation(errorC));
                addp("guard-continuation", new PGuardContinuation());
                addp("load", new PLoad());

                try
                {
                    var lst = Library.getLibrary();
                    var objs = Parser.ParseAll(lst);
                    foreach (KObject item in objs)
                    {
                        Evaluator.Eval(item, GroundEnv);
                    }
                }
                catch (ParseException e)
                {
                    Console.WriteLine("Library couldn't be loaded: " + e.Message + "\n");
                }
            }
            return GroundEnv;
        }
        private static void addp(string name, KOperative op)
        {
            GroundEnv.Bind(name, new KApplicative(op));
        }
        Dictionary<string, KObject> table = new Dictionary<string, KObject>();
        List<KEnvironment> parents = new List<KEnvironment>();
        public KEnvironment()
        {
        }
        public KEnvironment(KEnvironment parent)
        {
            parents.Add(parent);
        }
        public void AddParent(KEnvironment env)
        {
            parents.Add(env);
        }
        public KObject Lookup(string name)
        {
            KObject obj;
            bool found = table.TryGetValue(name, out obj);
            if (!found)
            {
                foreach (var env in parents)
                {
                    if (null == env)
                        break;
                    obj = env.Lookup(name);
                    if (obj is KObject)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found ? obj : null;
        }
        public void Bind(string name, KObject value)
        {
            if (table.ContainsKey(name))
                table[name] = value;
            else
                table.Add(name, value);
        }
        public override string Print(bool quoteStrings)
        {
            if (table.Count == 0 && parents.Count == 0)
            {
                return "#empty-env";
            } else {
                return "#<environment:" + String.Join(",", new List<string>(table.Keys)) + ">";
            }
        }
    }
}

