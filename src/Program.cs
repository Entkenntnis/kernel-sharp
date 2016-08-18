using System;
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
         */
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Kernel#harp! Call (exit) to quit.\n");

            Interpreter.LoadModule(new FastWrapModule());
            Interpreter.LoadModule(new CoreModule());
            Interpreter.LoadModule(new StringModule());
            Interpreter.LoadModule(new ConsoleModule());
            Interpreter.LoadModule(new ExceptionModule());
            Interpreter.LoadModule(new LoadModule());
            Interpreter.LoadModule(new TypesModule());
            Interpreter.LoadModule(new OperatorsModule());
            Interpreter.LoadModule(new EnvironmentModule());
            Interpreter.LoadModule(new ContinuationModule());
            Interpreter.LoadModule(new EncapsulationModule());
            Interpreter.LoadModule(new NumbersModule());

            // testing the interpreter
            /*args = new string[]{ "(load \"/home/dal/Schreibtisch/kernel\")" };

            foreach (string datum in args) {
                Interpreter.RunCode(datum);
            }*/
                
            Interpreter.REPL();
        }
    }
}
