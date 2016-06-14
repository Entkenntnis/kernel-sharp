﻿using System;
using System.Text;
using System.IO;

namespace Kernel
{
    class MainClass
    {
        /* 
         * This is a rough implementation of the Kernel language described by John Shutt.
         * http://web.cs.wpi.edu/~jshutt/kernel.html
         * 
         * The major "improvement" to Scheme is the raising of special forms to first-class,
         * generated by $vau and then able to compose most of other language elements.
         * 
         * The evaluation model is quite straightforward and therefore easy to implement.
         * Not so continuations. The biggest deviation may be the ommision of call/cc and Co.
         * They may be tackled in another project. Tail recursion is available, though.
         * 
         * Regarding the R-1RK draft, Chapter 0 to Chapter 6 has been completely implemented,
         * also Chapter 8 & 9. Against the definition this implementation is case-sensitive.
         * 
         * Only double (inexact) and long (exact) numbers are implemented and binary operations
         * on them - sufficient for most non-mathematical applications.
         * 
         * This is the proof of concept that a expressive and powerful language can be defined
         * in a simple and clear approach.
         * 
         */
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Kernel#harp! Call (exit) to quit.\n");
            KEnvironment env = new KEnvironment(KEnvironment.GetGroundEnv());

            args = new string[]{ "(load \"/home/dal/Schreibtisch/kernel\")" };

            if (args.Length > 0)
            {
                foreach (string datum in args)
                {
                    Evaluator.Eval(KObject.Parse(datum), env);
                }
            }
                
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
                    datum = KObject.Parse(sb.ToString());
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
