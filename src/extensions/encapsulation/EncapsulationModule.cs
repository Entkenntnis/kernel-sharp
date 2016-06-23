using System;

namespace Kernel
{
    public class EncapsulationModule : Module
    {
        public EncapsulationModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PEncap());
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

