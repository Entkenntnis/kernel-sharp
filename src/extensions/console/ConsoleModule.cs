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
            Interpreter.AddOp(new PDisplayln());
            Interpreter.AddOp(new PWriteln());
            Interpreter.AddOp(new PRead());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString()};
        }
    }
}

