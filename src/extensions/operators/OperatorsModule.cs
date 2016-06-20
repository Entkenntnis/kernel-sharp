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
            (#t (raise ""$and: lst of wrong type"")))))))

($define! $or
 (Y
  ($lambda (f)
    ($vau clauses env
        ($cond
            ((null? clauses) #f)
            ((pair? clauses) ($if (eval (car clauses) env)
                                  #t
                                  (eval (list* f (cdr clauses)) env)))
            (#t (raise ""$or: lst of wrong type"")))))))

($define! filter
 (Y
  ($lambda (f)
    ($lambda (pred lst)
        ($cond
            ((null? lst) ())
            ((pair? lst) ($if (pred (car lst))
                               (cons (car lst) (f pred (cdr lst)))
                               (f pred (cdr lst))))
            (#t (raise ""filter: lst of wrong type"")))))))

($define! foldl
 (Y
  ($lambda (f)
    ($lambda (proc init lst)
        ($cond
            ((null? lst) init)
            ((pair? lst) (f proc (proc (car lst) init) (cdr lst)))
            (#t (raise ""fold: lst of wrong type"")))))))

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
            (#t (raise ""reverse: lst of wrong type"")))))

($define! for-each
 (Y
  ($lambda (f)
    ($lambda (proc lst)
        ($cond
            ((null? lst) #inert)
            ((pair? lst) (proc (car lst))
                         (f proc (cdr lst)))
            (#t (raise ""for-each: lst of wrong type"")))))))

($define! andmap
    ($vau (proc lst) env
        (eval (cons $and (map ($lambda (x) (list proc x)) (eval lst env))) env)))

($define! ormap
    ($vau (proc lst) env
        (eval (cons $or (map ($lambda (x) (list proc x)) (eval lst env))) env)))

($define! assoc
 (Y
  ($lambda (f)
    ($lambda (v lst)
        ($cond
            ((null? lst) #f)
            ((equal? (caar lst) v)
                (car lst))
            (#t (f v (cdr lst))))))))

($define! member?
 (Y
  ($lambda (f)
    ($lambda (v lst)
        ($cond
            ((null? lst) #f)
            ((equal? v (car lst)) #t)
            (#t (f v (cdr lst))))))))

($define! append2
 (Y
  ($lambda (f)
    ($lambda (a b)
        ($cond
            ((null? a) b)
            ((null? b) a)
            ((pair? a) (cons (car a) (f (cdr a) b)))
            (#t (raise ""append2: lst of wrong type22)))))))

($define! append
 (Y
  ($lambda (f)
    ($lambda lsts
        ($cond
            ((null? lsts) ())
            ((null? (cdr lsts)) (car lsts))
            (#t (append2 (car lsts) (apply f (cdr lsts)))))))))


            ";
        }
    }
}

