using System;
using System.Collections.Generic;
using System.IO;

namespace Kernel
{
    public class PLoad : POperative
    {
        public PLoad()
        {
        }

        public override string getName()
        {
            return "load";
        }

        public override object Do(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            CPara(args, 1);
            KString path = First(args) as KString;
            Check(path);
            try
            {
                List<KObject> tokens = Parser.ParseAll(File.ReadAllText(path.Value));
                foreach (var token in tokens)
                {
                    Evaluator.Eval(token, env);
                }
                return new KInert();
            }
            catch (Exception e)
            {
                throw new RuntimeException("Failed to load file: " + e.Message);
            }
        }
    }
}

