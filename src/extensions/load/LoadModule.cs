using System;

namespace Kernel
{
    public class LoadModule : Module
    {
        public LoadModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PLoad());
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString()};
        }
    }
}

