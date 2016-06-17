using System;

namespace Kernel
{
    public class CoreModule : Module
    {
        public CoreModule()
        {
        }

        public override void Init()
        {
            Interpreter.ExtendGroundEnv("equal?", new KApplicative(new PEqual()));
            Interpreter.ExtendGroundEnv("cons", new KApplicative(new PCons()));
            Interpreter.ExtendGroundEnv("eval", new KApplicative(new PEval()));
            Interpreter.ExtendGroundEnv("$if", new PIf());
            Interpreter.ExtendGroundEnv("$define!", new PDefine());
            Interpreter.ExtendGroundEnv("$vau", new PVau());

            Interpreter.LoadLibrary(new CoreLibrary());
        }
    }
}

