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
    ($vau clauses env
        ($cond
            ((null? clauses) #t)
            ((pair? clauses) ($if (eval (car clauses) env)
                                  (eval (list* $and (cdr clauses)) env)
                                  #f))
            (#t (raise ""$and: lst of wrong type"")))))

($define! $or
    ($vau clauses env
        ($cond
            ((null? clauses) #f)
            ((pair? clauses) ($if (eval (car clauses) env)
                                  #t
                                  (eval (list* $or (cdr clauses)) env)))
            (#t (raise ""$or: lst of wrong type"")))))

($define! filter
    ($lambda (pred lst)
        ($cond
            ((null? lst) ())
            ((pair? lst) ($if (pred (car lst))
                               (cons (car lst) (filter pred (cdr lst)))
                               (filter pred (cdr lst))))
            (#t (raise ""filter: lst of wrong type"")))))

($define! foldl
    ($lambda (proc init lst)
        ($cond
            ((null? lst) init)
            ((pair? lst) (foldl proc (proc (car lst) init) (cdr lst)))
            (#t (raise ""fold: lst of wrong type"")))))

($define! reverse
    ($lambda (lst)
        ($cond
            ((null? lst) ())
            ((pair? lst) 
                ($define! walk
                    ($lambda (input acc)
                        ($let ((next (cons (car input) acc)))
                          ($cond
                            ((null? (cdr input)) next)
                            (#t (walk (cdr input) next))))))
                  (walk lst ()))
            (#t (raise ""reverse: lst of wrong type"")))))

($define! for-each
    ($lambda (proc lst)
        ($cond
            ((null? lst) #inert)
            ((pair? lst) (proc (car lst))
                         (for-each proc (cdr lst)))
            (#t (raise ""for-each: lst of wrong type"")))))

($define! andmap
    ($vau (proc lst) env
        (eval (cons $and (map ($lambda (x) (list proc x)) (eval lst env))) env)))

($define! ormap
    ($vau (proc lst) env
        (eval (cons $or (map ($lambda (x) (list proc x)) (eval lst env))) env)))

($define! assoc
    ($lambda (v lst)
        ($cond
            ((null? lst) #f)
            ((equal? (caar lst) v)
                (car lst))
            (#t (assoc v (cdr lst))))))

($define! member?
    ($lambda (v lst)
        ($cond
            ((null? lst) #f)
            ((equal? v (car lst)) #t)
            (#t (member? v (cdr lst))))))

($define! append
    ($lambda lsts
      ($define! append2
        ($lambda (a b)
            ($cond
                ((null? a) b)
                ((null? b) a)
                ((pair? a) (cons (car a) (append2 (cdr a) b)))
                (#t (raise ""append: lst of wrong type"")))))
        ($cond
            ((null? lsts) ())
            ((null? (cdr lsts)) (car lsts))
            (#t (append2 (car lsts) (apply append (cdr lsts)))))))


            ";
        }
    }
}

