using System;

namespace Kernel
{
    public class EnvironmentModule : Module
    {
        public EnvironmentModule()
        {
        }

        public override void Init()
        {
            
        }

        public override string[] DependOn()
        {
            return new string[]{new CoreModule().ToString(), new OperatorsModule().ToString()
                , new ExceptionModule().ToString()};
        }

        public override string getLibrary()
        {
            return @"

($define! $binds?
    ($vau (exp . ss) dynamic
        ($handle ($lambda #ignore #f)
            ($let ((env (eval exp dynamic)))
                (map ($lambda (sym) (eval sym env)) ss)
                #t))))

($define! get-current-environment (wrap ($vau () e e)))

($define! make-standard-environment
    ($lambda () (get-current-environment)))

($define! $let*
    ($vau (bindings . body) env
        (eval ($if (null? bindings)
                   (list* $let bindings body)
                   (list $let
                    (list (car bindings))
                    (list* $let* (cdr bindings) body)))
        env)))

($define! $letrec
    ($vau (bindings . body) env
        (eval (list* $let ()
                     (list $define!
                        (map car bindings)
                        (list* list (map cadr bindings)))
                     body)
        env)))

($define! $letrec*
    ($vau (bindings . body) env
        (eval ($if (null? bindings)
                   (list* $letrec bindings body)
                   (list $letrec
                    (list (car bindings))
                    (list* $letrec* (cdr bindings) body)))
        env)))

($define! $let-redirect
    ($vau (exp bindings . body) env
        (eval (list* (eval (list* $lambda (map car bindings) body)
                        (eval exp env))
                     (map cadr bindings))
        env)))

($define! $let-safe
    ($vau (bindings . body) env
        (eval (list* $let-redirect
                     (make-standard-environment)
                     bindings
                     body)
        env)))

($define! $remote-eval
    ($vau (o e) d
        (eval o (eval e d))))

($define! $bindings->environemnt
    ($vau bindings denv
        (eval (list $let-redirect
                    #empty-env
                    bindings
                    (list get-current-environment))
        denv)))

($define! $set!
    ($vau (exp1 formals exp2) env
        (eval (list $define! formals
                (list eval (list ($vau (x) #ignore x) exp2) env))
              (eval exp1 env))))

($define! $provide!
    ($vau (symbols . body) env
        (eval (list $define! symbols
            (list $let ()
                (list* $sequence body)
                (list* list symbols)))
          env)))

($define! $import!
    ($vau (exp . symbols) env
        (eval (list $set!
                    env
                    symbols
                    (cons list symbols))
            eval exp env)))


            ";
        }
    }
}

