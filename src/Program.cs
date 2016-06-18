﻿using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

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

            Interpreter.LoadModule(new CoreModule());
            Interpreter.LoadModule(new StringModule());
            Interpreter.LoadModule(new ConsoleModule());
            Interpreter.LoadModule(new NumberModule());
            Interpreter.ExtendGroundEnv("handle", new PHandler());

            // testing the interpreter
            args = new string[]{ "($define! y (map cons ($quote (a b c)) ($quote (d e f))))" };

            foreach (string datum in args) {
                Interpreter.RunCode(datum);
            }
                
            Interpreter.REPL();
        }
    }
}
