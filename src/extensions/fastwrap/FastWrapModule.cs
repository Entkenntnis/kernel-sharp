using System;

namespace Kernel
{
    public class FastWrapModule : Module
    {

    // To use this module load it as the first. The core library can override the wrap function

        public FastWrapModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PWrap());
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

