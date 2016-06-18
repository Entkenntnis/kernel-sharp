using System;

namespace Kernel
{
    public class OperatorsModule : Module
    {
        public OperatorsModule()
        {
        }

        public override void Init()
        {
            
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new StringModule().ToString(),
                new ExceptionModule().ToString(), new TypesModule().ToString()};
        }

        public override string getLibrary()
        {
            return @"
($define! Y
    ($lambda (f)
        ($let ((rec ($lambda (g) (f ($lambda x (apply (g g) x))))))
            (rec rec))))
        

($define! $unless
    ($vau (pred . body) env
        ($if (eval pred env)
            #inert
            (eval (cons $sequence body) env))))      

($define! not
    ($lambda (x)
        ($if x #f #t)))          

($define! $when
    ($vau (pred . body) env
        (eval (list* $unless (list not pred) body) env)))


($define! $and
 (Y
  ($lambda (f)
    ($vau clauses env
        ($cond
            ((null? clauses) #t)
            ((pair? clauses) ($if (eval (car clauses) env)
                                  (eval (list* f (cdr clauses)) env)
                                  #f))
            (#t (raise ""$and: lst wrong type"")))))))

($define! $or
 (Y
  ($lambda (f)
    ($vau clauses env
        ($cond
            ((null? clauses) #f)
            ((pair? clauses) ($if (eval (car clauses) env)
                                  #t
                                  (eval (list* f (cdr clauses)) env)))
            (#t (raise ""$or: lst wrong type"")))))))

($define! filter
 (Y
  ($lambda (f)
    ($lambda (pred lst)
        ($cond
            ((null? lst) ())
            ((pair? lst) ($if (pred (car lst))
                               (cons (car lst) (f pred (cdr lst)))
                               (f pred (cdr lst))))
            (#t (raise ""filter: lst wrong type"")))))))

($define! foldl
 (Y
  ($lambda (f)
    ($lambda (proc init lst)
        ($cond
            ((null? lst) init)
            ((pair? lst) (f proc (proc (car lst) init) (cdr lst)))
            (#t (raise ""fold: lst wrong type"")))))))

($define! reverse
    ($lambda (lst)
        ($cond
            ((null? lst) ())
            ((pair? lst) 
                ($let ((walk (Y
                  ($lambda (f)
                    ($lambda (input acc)
                        ($let ((next (cons (car input) acc)))
                          ($cond
                            ((null? (cdr input)) next)
                            (#t (f (cdr input) next)))))))))
                    (walk lst ())))
            (#t (raise ""reverse: lst wrong type"")))))





























            ";
        }
    }
}

