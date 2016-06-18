using System;

namespace Kernel
{
    public class ConsoleModule : Module
    {
        public ConsoleModule()
        {
        }

        public override void Init()
        {
            Interpreter.ExtendGroundEnv("display", new KApplicative(new PDisplay()));
            Interpreter.ExtendGroundEnv("write", new KApplicative(new PWrite()));
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString()};
        }
    }
}

