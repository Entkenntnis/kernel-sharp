using System;

namespace Kernel
{
    public class ExceptionModule : Module
    {
        public ExceptionModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PHandle());
            Interpreter.AddOp(new PRaise());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString()};
        }

        public override string getLibrary()
        {
            return 

            @"
($define! $handle
    ($let ((old-handle $handle))
        ($vau (h . body) env
            (eval (list old-handle h (cons $sequence body)) env))))


            ";
        }
    }
}

