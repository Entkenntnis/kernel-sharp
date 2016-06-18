using System;

namespace Kernel
{
    public class TypesModule : Module
    {
        public TypesModule()
        {
        }

        public override void Init()
        {
            Interpreter.AddOp(new PNullp());
            Interpreter.AddOp(new PBooleanp());
            Interpreter.AddOp(new PSymbolp());
            Interpreter.AddOp(new PPairp());
            Interpreter.AddOp(new PInertp());
            Interpreter.AddOp(new PIgnorep());
            Interpreter.AddOp(new PEnvironmentp());
            Interpreter.AddOp(new PCombinerp());
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

