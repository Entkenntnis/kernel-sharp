using System;
using System.Collections.Generic;

namespace Kernel
{
    public abstract class Module
    {
        public abstract void Init();

        public abstract string[] DependOn();
    }
}

