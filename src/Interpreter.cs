using System;
using System.Text;

namespace Kernel
{
    public class Interpreter
    {
        KEnvironment env;

        public Interpreter()
        {
            env = new KEnvironment(KEnvironment.GetGroundEnv());
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

