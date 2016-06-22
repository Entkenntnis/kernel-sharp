using System;

namespace Kernel
{
    public class ContinuationModule : Module
    {
        public ContinuationModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PCallCC());
            Interpreter.AddOp(new PContinuation2Applicative());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString()};
        }

        public override string getLibrary()
        {
            return @"
($define! apply-continuation
    ($lambda (c o)
        (apply (continuation->applicative c) o)))

($define! $let/cc
    ($vau (symbol . body ) env
        (eval (list call/cc (list* $lambda (list symbol) body))
            env)))

            ";
        }
    }
}

