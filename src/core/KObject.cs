using System;
using System.Collections.Generic;
using System.Text;

/* This file contains definitions of all necessary objects for the interpreter core
*/

namespace Kernel
{

    public abstract class KObject
    {
        public string Write()
        {
            return this.Print(true);
        }

        public string Display()
        {
            return this.Print(false);
        }

        public abstract string Print(bool quoteStrings);

        public virtual string Print(bool quoteStrings, List<KObject> visited)
        {
            return Print(quoteStrings);
        }

        public override string ToString()
        {
            return Write();
        }
    }






}

