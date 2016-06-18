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
    }
}

