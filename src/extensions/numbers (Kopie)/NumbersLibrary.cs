using System;

namespace Kernel
{
    public class NumbersLibrary : Library
    {
        public override string getLibrary()
        {
            return 
            @"
($define! infinity?
    ($lambda (x)
        ($if (double? x)
            ($if (equal? x #e+infinity)
                #t
                ($if (equal? x #e-infinity)
                    #t #f)) #f)))

($define! round
    ($lambda (x)
        ($cond
            ((infinity? x) x)
            ((integer? x) x)
            (#t (double->integer (double-round x))))))


            ";
        }
    }
}

