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
            Interpreter.AddOp(new PEqual());
            Interpreter.AddOp(new PCons());
            Interpreter.AddOp(new PEval());
            Interpreter.AddOp(new PIf());
            Interpreter.AddOp(new PDefine());
            Interpreter.AddOp(new PVau());

            Interpreter.LoadLibrary(new CoreLibrary());
        }

        public override string[] DependOn()
        {
            return new string[]{};
        }
    }
}

