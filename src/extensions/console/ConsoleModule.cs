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
            Interpreter.AddOp(new PDisplay());
            Interpreter.AddOp(new PWrite());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString()};
        }
    }
}

