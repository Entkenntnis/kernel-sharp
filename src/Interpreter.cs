using System;
using System.Text;

namespace Kernel
{
    public class Interpreter
    {
        private static bool initialized = false;
        private static KEnvironment env;

        private static void init()
        {
            if (initialized)
                return;
            env = new KEnvironment(GetGroundEnv());
            LoadLibrary(new CoreLibrary());
            initialized = true;
        }

        private static KEnvironment GroundEnv = null;

        private static KEnvironment GetGroundEnv()
        {
            if (null == GroundEnv) {
                GroundEnv = new KEnvironment(null);
                addp("equal?", new PEqual());
                addp("cons", new PCons());
                addp("eval", new PEval());
                GroundEnv.Bind("$if", new PIf());
                GroundEnv.Bind("$define!", new PDefine());
                GroundEnv.Bind("$vau", new PVau());


            }
            return GroundEnv;
        }

        private static void addp(string name, KOperative op)
        {
            GroundEnv.Bind(name, new KApplicative(op));
        }

        public static void ExtendGroundEnv(string symbol, KObject value)
        {
            init();
            GetGroundEnv().Bind(symbol, value);
        }

        public static void LoadLibrary(Library lib)
        {
            try {
                var lst = lib.getLibrary();
                var objs = Parser.ParseAll(lst);
                foreach (KObject item in objs) {
                    Evaluator.Eval(item, GroundEnv);
                }
            } catch (ParseException e) {
                Console.WriteLine("Library couldn't be loaded: " + e.Message + "\n");
            }
        }

        public static void LoadModule(Module mod)
        {
            mod.Init();
        }

        public static KObject RunCode(string datum)
        {
            init();
            return Evaluator.Eval(Parser.Parse(datum), env);
        }

        public static void REPL()
        {
            init();
            while (true) {
                Console.Write(">> ");
                KObject datum = null;
                try {
                    StringBuilder sb = new StringBuilder();
                    bool finished = false;
                    bool quoteContext = false;
                    int parenCount = 0;
                    while (!finished) {
                        string inData = Console.ReadLine() + "\n";
                        for (int i = 0; i < inData.Length; i++) {
                            if (inData[i] == ';')
                                break;
                            if (inData[i] == '"') {
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
                } catch (ParseException e) {
                    Console.WriteLine("ParseError: {0}", e.Message);
                    continue;
                }
                if (datum is KPair) {
                    KPair p = datum as KPair;
                    if (p.Car is KSymbol && ((KSymbol)p.Car).Value.Equals("exit") && p.Cdr is KNil)
                        break;
                }
                try {
                    datum = Evaluator.Eval(datum, env);
                    Console.WriteLine("\n" + datum.Write() + "\n");
                } catch (RuntimeException e) {
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

