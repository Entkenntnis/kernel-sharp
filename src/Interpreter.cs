using System;
using System.Text;

namespace Kernel
{
    public class Interpreter
    {
        KEnvironment env;

        private static KEnvironment GroundEnv = null;
        public static KEnvironment GetGroundEnv()
        {
            if (null == GroundEnv)
            {
                GroundEnv = new KEnvironment(null);
                //addp("boolean?", new PBoolean());
                addp("equal?", new PEqual());
                //addp("eq?", new PEq());
                addp("equal?", new PEqual());
                //addp("symbol?", new PSymbol());
                //addp("inert?", new PInert());
                //addp("pair?", new PPair());
                //addp("null?", new PNull());
                //addp("environment?", new PEnvironment());
                //addp("ignore?", new PIgnore());
                //addp("operative?", new POperative());
                //addp("applicative?", new PApplicative());
                addp("cons", new PCons());
                addp("eval", new PEval());
                //addp("make-environment", new PMakeEnvironment());
                //addp("set-car!", new PSetCar());
                //addp("set-cdr!", new PSetCdr());
                //addp("copy-es-immutable", new PCopyEsImmutable());
                //addp("copy-es", new PCopyEs());
                addp("wrap", new PWrap());
                //addp("unwrap", new PUnwrap());
                GroundEnv.Bind("$if", new PIf());
                GroundEnv.Bind("$define!", new PDefine());
                GroundEnv.Bind("$vau", new PVau());
                addp("+", new PAdd());
                addp("-", new PSub());
                addp("*", new PMult());
                addp("/", new PDiv());
                //addp("number?", new PNumber());
                //addp("integer?", new PInteger());
                //addp("=?", new PNEq());
                //addp("<?", new PNless());
                //addp(">?", new PNgreater());
                //addp("inexact?", new PInexact());
                addp("write", new PWrite());
                addp("display", new PDisplay());
                //addp("make-encapsulation-type", new PEncap());
                //addp("continuation?", new PContinuation());
                //addp("call/cc", new PCallCC());
                //addp("continuation->applicative", new PContinuationToApplicative());
                //addp("extend-continuation", new PExtendContinuatione());
                //GroundEnv.Bind("root-continuation", new KContinuation(CPS.RootContinuation<KObject>()));
                //var errorC = CPS.RootContinuation<KObject>();
                //errorC.isError = true;
                //GroundEnv.Bind("error-continuation", new KContinuation(errorC));
                //addp("guard-continuation", new PGuardContinuation());
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

        public Interpreter()
        {
            env = new KEnvironment(GetGroundEnv());
        }

        public KObject RunCode(string datum)
        {
            return Evaluator.Eval(Parser.Parse(datum), env);
        }

        public void REPL()
        {
            while (true)
            {
                Console.Write(">> ");
                KObject datum = null;
                try
                {
                    StringBuilder sb = new StringBuilder();
                    bool finished = false;
                    bool quoteContext = false;
                    int parenCount = 0;
                    while (!finished)
                    {
                        string inData = Console.ReadLine() + "\n";
                        for(int i = 0; i < inData.Length; i++)
                        {
                            if (inData[i] == ';')
                                break;
                            if (inData[i] == '"')
                            {
                                int previous = i - 1;
                                while (previous > 0 && inData[previous] == '\\')
                                    previous--;
                                if (i - 1 - previous % 2 == 1)
                                    quoteContext = !quoteContext;
                            }
                            if (inData[i] == '(' && !quoteContext)
                                parenCount++;
                            else if (inData[i] == ')' && !quoteContext)
                                parenCount--;
                            if (parenCount < 0)
                                throw new ParseException("Unbalanced parenthesis!");
                        }
                        sb.Append(inData);
                        if (parenCount == 0)
                            finished = true;
                    }
                    datum = Parser.Parse(sb.ToString());
                }
                catch (ParseException e)
                {
                    Console.WriteLine("ParseError: {0}", e.Message);
                    continue;
                }
                if (datum is KPair)
                {
                    KPair p = datum as KPair;
                    if (p.Car is KSymbol && ((KSymbol)p.Car).Value.Equals("exit") && p.Cdr is KNil)
                        break;
                }
                try
                {
                    datum = Evaluator.Eval(datum, env);
                    Console.WriteLine("\n" + datum.Write() + "\n");
                }
                catch (RuntimeException e)
                {
                    Console.WriteLine("RuntimeException: " + e.Message);
                }
                /*catch (Exception e)
                {
                    Console.WriteLine("Something went really wrong: " + e.Message);
                }*/

            }
        }
    }
}

