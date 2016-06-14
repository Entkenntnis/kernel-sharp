﻿using System;

namespace Kernel
{
    public class Library
    {
        public static string getLibrary()
        {
            return 
/*
obsolete version of sequence definition
replaced by version without explicit recursion
($define! $xxxsequence
   ((wrap ($vau ($seq2) #ignore
             ($seq2
                ($define! $aux
                   ($vau (head . tail) env
                      ($if (null? tail)
                           (eval head env)
                           ($seq2
                              (eval head env)
                              (eval (cons $aux tail) env)))))
                ($vau body env
                   ($if (null? body)
                        #inert
                        (eval (cons $aux body) env))))))

      ($vau (first second) env
         ((wrap ($vau #ignore #ignore (eval second env)))
          (eval first env)))))


also replaced with version avoiding explicit recursion
($define! list*
   (wrap ($vau args #ignore
            ($sequence
               ($define! aux
                  (wrap ($vau ((head . tail)) #ignore
                           ($if (null? tail)
                                head
                                (cons head (aux tail))))))
               (aux args)))))


this was more tricky but it worked out well
($define! $cond
   ($vau clauses env

      ($define! aux
         ($lambda ((test . body) . clauses)
            ($if (eval test env)
                 (apply (wrap $sequence) body env)
                 (apply (wrap $cond) clauses env))))

      ($if (null? clauses)
           #inert
           (apply aux clauses))))

this was simple
($define! get-list-metrics
   ($lambda (ls)

      ($define! aux
         ($lambda (kth k nth n)
            ($if (>=? k n)
                 ($if (pair? (cdr nth))
                      (aux ls 0 (cdr nth) (+ n 1))
                      (list (+ n 1)
                            ($if (null? (cdr nth)) 1 0)
                            (+ n 1)
                            0))
                 ($if (eq? kth nth)
                      (list n 0 k (- n k))
                      (aux (cdr kth) (+ k 1) nth n)))))

      ($if (pair? ls)
           (aux ls 0 ls 0)
           (list 0 ($if (null? ls) 1 0) 0 0))))


this was easy: standard technique
($define! list-tail
   ($lambda (ls k)
      ($if (>? k 0)
           (list-tail (cdr ls) (- k 1))
           ls)))
making this more atomic, avoid $lambda

($define! car ($lambda ((x . #ignore)) x))
($define! cdr ($lambda ((#ignore . x)) x))

*/

@"

($define! wrap1
  ($vau (op) env
    ($vau (arg) a-env
      (eval (cons op (cons (eval arg a-env) ())) env))))

($define! car
  (wrap1 ($vau ((x . #ignore)) #ignore x)))
 
($define! cdr
  (wrap1 ($vau ((#ignore . x)) #ignore x)))

($define! $quote ($vau (x) #ignore x))

; library implementation of wrap. Functional, but very slow. Currently disabled.
($define! xwrap
  ($vau (op) env
    ($vau args a-env
      ((wrap1 ($vau #ignore #ignore
        (eval (cons op (walk args)) env)
        ))
        ($define! walk
          (wrap1 ($vau (args) #ignore
            ($if (null? args)
              ()
              (cons (eval (car args) a-env)
                (walk (cdr args))))
            )))
        ))))

; this is not working AND not used anywhere anymore ...
; so don't care for it (but: take care of $set! later)
;($define! xunwrap
;  ($vau (app) env
;    ($vau args #ignore
;      ((wrap1 ($vau #ignore #ignore
;        (eval (cons app (walk args)) env)
;        ))
;        ($define! walk
;          (wrap1 ($vau (args) e
;            ($if (null? args)
;              ()
;              (cons (cons $quote (cons (car args) ()))
;                (walk (cdr args)))))))))))

($define! $sequence
    ((wrap ($vau ($seq2) #ignore
        ((wrap ($vau ($rec) #ignore
            ($rec $rec))) ;this substitutes a y combinator
         ($vau (f) c-env
            ($vau (head . tail) env
                ($if (null? tail)
                    (eval head env)
                    ($seq2
                        (eval head env)
                            (eval (cons ((eval f c-env) (eval f c-env))
                                        tail) env))))))))
     ($vau (first second) env
        ((wrap ($vau #ignore #ignore (eval second env)))
         (eval first env)))))


         (write ($quote seq-loaded))

($define! list (wrap ($vau x #ignore x)))

($define! list*
  ((wrap ($vau (my-list-rec) #ignore
    (wrap ($vau args #ignore
        ((my-list-rec my-list-rec) args)))))
   (wrap ($vau (f) #ignore
      (wrap ($vau ((head . tail)) #ignore
          ($if (null? tail)
              head
              (cons head ((f f) tail)))))))))

              ($define! $vau1 $vau)

($define! $vau
             ($vau1 (formals eformal . body) env
                (eval (cons $vau1 (cons formals (cons eformal
                           (cons (cons $sequence body)()))))
                      env)))

      (write ($quote $vau-redefined))

($define! $lambda
   ($vau (formals . body) env
      (wrap (eval (list* $vau formals #ignore body)
                  env))))

                  (write ($quote $lambda-defined))


;($define! car ($lambda ((x . #ignore)) x))
;($define! cdr ($lambda ((#ignore . x)) x))

($define! caar ($lambda (((x . #ignore) . #ignore)) x))
($define! cdar ($lambda (((#ignore . x) . #ignore)) x))
($define! cadr ($lambda ((#ignore . (x . #ignore))) x))
($define! cddr ($lambda ((#ignore . (#ignore . x))) x))

($define! caaar ($lambda ((((x . #ignore) . #ignore) . #ignore)) x))
($define! cdaar ($lambda ((((#ignore . x) . #ignore) . #ignore)) x))
($define! cadar ($lambda (((#ignore . (x . #ignore)) . #ignore)) x))
($define! cddar ($lambda (((#ignore . (#ignore . x)) . #ignore)) x))
($define! caadr ($lambda ((#ignore . ((x . #ignore) . #ignore))) x))
($define! cdadr ($lambda ((#ignore . ((#ignore . x) . #ignore))) x))
($define! caddr ($lambda ((#ignore . (#ignore . (x . #ignore)))) x))
($define! cdddr ($lambda ((#ignore . (#ignore . (#ignore . x)))) x))

($define! caaaar ($lambda (((((x . #ignore) . #ignore) . #ignore). #ignore))x))
($define! cdaaar ($lambda (((((#ignore . x) . #ignore) . #ignore). #ignore))x))
($define! cadaar ($lambda ((((#ignore . (x . #ignore)) . #ignore). #ignore))x))
($define! cddaar ($lambda ((((#ignore . (#ignore . x)) . #ignore). #ignore))x))
($define! caadar ($lambda (((#ignore . ((x . #ignore) . #ignore)). #ignore))x))
($define! cdadar ($lambda (((#ignore . ((#ignore . x) . #ignore)). #ignore))x))
($define! caddar ($lambda (((#ignore . (#ignore . (x . #ignore))). #ignore))x))
($define! cdddar ($lambda (((#ignore . (#ignore . (#ignore . x))). #ignore))x))
($define! caaadr ($lambda ((#ignore .(((x . #ignore) . #ignore) . #ignore)))x))
($define! cdaadr ($lambda ((#ignore .(((#ignore . x) . #ignore) . #ignore)))x))
($define! cadadr ($lambda ((#ignore .((#ignore . (x . #ignore)) . #ignore)))x))
($define! cddadr ($lambda ((#ignore .((#ignore . (#ignore . x)) . #ignore)))x))
($define! caaddr ($lambda ((#ignore .(#ignore . ((x . #ignore) . #ignore))))x))
($define! cdaddr ($lambda ((#ignore .(#ignore . ((#ignore . x) . #ignore))))x))
($define! cadddr ($lambda ((#ignore .(#ignore . (#ignore . (x . #ignore)))))x))
($define! cddddr ($lambda ((#ignore .(#ignore . (#ignore . (#ignore . x)))))x))

($define! apply
  (($lambda (helper)
     ($lambda (appv arg . opt)
        (eval (cons appv ((helper helper) arg))
              ($if (null? opt)
                   (make-environment)
                   (car opt)))))
   ($lambda (f)
    ($lambda (lst)
      ($if (null? lst)
          ()
          (cons (list $quote (car lst))
                ((f f) (cdr lst))))))))


($define! $cond
  (($lambda (my-cond-rec)
      (my-cond-rec my-cond-rec))
  ($lambda (f)
    ($vau clauses env
      ($if (null? clauses)
           #inert
           (apply
              ($lambda ((test . body) . clauses)
                  ($if (eval test env)
                        (eval (cons $sequence body) env)
                        (eval (cons (f f) clauses) env)))
               clauses))))))

($define! <=?
    ($lambda (a b) ($if (=? a b) #t ($if (<? a b) #t #f))))

($define! >=?
    ($lambda (a b) ($if (=? a b) #t ($if (>? a b) #t #f))))


($define! get-list-metrics
   ($lambda (ls)
    (($lambda (aux-rec)
      ($if (pair? ls)
           ((aux-rec aux-rec) ls 0 ls 0)
           (list 0 ($if (null? ls) 1 0) 0 0)))
    ($lambda (f)
     ($lambda (kth k nth n)
        ($if (>=? k n)
             ($if (pair? (cdr nth))
                  ((f f) ls 0 (cdr nth) (+ n 1))
                  (list (+ n 1)
                        ($if (null? (cdr nth)) 1 0)
                        (+ n 1)
                        0))
             ($if (eq? kth nth)
                  (list n 0 k (- n k))
                  ((f f) (cdr kth) (+ k 1) nth n))))))))
                 

($define! list-tail
  (($lambda (list-tail-rec)
      (list-tail-rec list-tail-rec))
  ($lambda (f)
   ($lambda (ls k)
      ($if (>? k 0)
           ((f f) (cdr ls) (- k 1))
           ls)))))

($define! encycle!
   ($lambda (ls k1 k2)
      ($if (>? k2 0)
           (set-cdr! (list-tail ls (+ (+ k1 k2) -1))
                     (list-tail ls k1))
           #inert)))

;
; digression:
;   math applicatives max and lcm are used by map, so must be provided
;   to test map, and without using anything derived later than map
;

($define! max
  ($lambda x
    (($lambda (aux)
      (apply (aux aux)
          (list* (car (get-list-metrics x)) #e-infinity x)))
    ($lambda (f)
      ($lambda (count result . x)
        ($if (<=? count 0)
              result
              (apply (f f) (list* (- count 1) 
                                  ($if (>? (car x) result)
                                    ($if (inexact? result)
                                        (* (car x) 1.0)
                                        (car x))
                                    ($if (inexact? (car x))
                                        (* result 1.0)
                                        result)) 
                                  (cdr x)))))))))

($define! abs
  ($lambda (x)
    ($if (>=? x 0)
      x
      (* -1 x))))

($define! lcm
  ($lambda x
    (($lambda (gcd)
      (($lambda (aux3)
        (($lambda (aux2)
          (($lambda (aux)
            (apply (aux aux)
              (list* (car (get-list-metrics x)) 1 x)))
          ($lambda (f)
            ($lambda (count result . x)
              ($if (<=? count 0)
                  result
                  (apply (f f) (list* (- count 1)
                      (aux2 result (car x))
                      (cdr x))))))))
        ($lambda (result k)
          ($cond ((=? k 0)                 (* k #e+infinity)) ; induce error
                 ((=? k #e+infinity)       (* k result))
                 ((=? k #e-infinity)       (* k result -1))
                 ((=? result #e+infinity)  (* result (abs k)))
                 (#t                       (aux3 result (abs k)))))))
      ($lambda (x y)
        (/ (* x y) ((gcd gcd) x y)))))
    ($lambda (f)
      ($lambda (x y)
        ($if (=? x y)
            x             ; don't worry here about inexactness
            ($if (<? x y)
              ((f f) x (- y x))
              ((f f) (- x y) y))))))))

;
; now, back to core derivations
;

($define! map
   (wrap ($vau (appv . lss) env

      ($define! acc
         ($lambda (input (k1 k2) base-result head tail sum)
            ($define! aux
               ($lambda (input count)
                  ($if (=? count 0)
                       base-result
                       (sum (head input)
                            (aux (tail input) (- count 1))))))
            (aux input (+ k1 k2))))

      ($define! enlist
         ($lambda (input ms head tail)
            ($define! result (acc input ms () head tail cons))
            (apply encycle! (list* result ms))
            result))

      ($define! mss (cddr (get-list-metrics lss)))

      ($define! cars ($lambda (lss) (enlist lss mss caar cdr)))
      ($define! cdrs ($lambda (lss) (enlist lss mss cdar cdr)))

      ($define! result-metrics
         (acc lss mss (cddr (get-list-metrics (car lss)))
              ($lambda (lss) (cddr (get-list-metrics (car lss))))
              cdr
              ($lambda ((j1 j2) (k1 k2))
                 (list (max j1 k1)
                       ($cond ((=? j2 0)  k2)
                              ((=? k2 0)  j2)
                              (#t  (lcm j2 k2)))))))

      (enlist lss
              result-metrics
              ($lambda (lss) (apply appv (cars lss) env))
              cdrs))))

($define! $let
   ($vau (bindings . body) env
      (eval (cons (list* $lambda (map car bindings) body)
                  (map cadr bindings))
            env)))

($define! not? ($lambda (x) ($if x #f #t)))

($define! and?
   ($lambda x

      ($define! aux
         ($lambda (x k)
            ($cond ((<=? k 0)  #t)
                   ((car x)    (aux (cdr x) (- k 1)))
                   (#t         #f))))

      (aux x (car (get-list-metrics x)))))

($define! or?
   ($lambda x
      (not? (apply and? (map not? x)))))

($define! $and?
   ($vau x e
      ($cond ((null? x)         #t)
             ((null? (cdr x))   (eval (car x) e)) ; tail context
             ((eval (car x) e)  (apply (wrap $and?) (cdr x) e))
             (#t                #f))))

($define! $or?
   ($vau x e
      ($cond ((null? x)         #f)
             ((null? (cdr x))   (eval (car x) e)) ; tail context
             ((eval (car x) e)  #t)
             (#t                (apply (wrap $or?) (cdr x) e)))))

($define! combiner?
   ($lambda x
      (apply and? (map ($lambda (x)
                          (or? (applicative? x)
                               (operative? x)))
                       x))))

($define! length
   ($lambda (object)
      ($let (((#ignore #ignore a c)  (get-list-metrics object)))
         ($if (>? c 0)
              %e+infinity
              a))))

($define! list-ref
   ($lambda (ls k)
      (car (list-tail ls k))))

($define! append
   ($lambda lss

      ($define! set-last!
         ($lambda (ls tail) ; set cdr of last pair of ls to tail
            ($let ((next  (cdr ls)))
               ($if (pair? next)
                    (set-last! next tail)
                    (set-cdr! ls tail)))))

      ($define! aux2
         ($lambda (ls tail) ; prepend ls onto tail
            ($if (null? ls)
                 tail
                 (cons (car ls) (aux2 (cdr ls) tail)))))

      ($define! aux1
         ($lambda (k lss tail) ; prepend k elmts of lss onto tail
            ($if (>? k 0)
                 (aux2 (car lss)
                       (aux1 (- k 1) (cdr lss) tail))
                 tail)))

      ($if (null? lss)
           ()
           ($let (((#ignore #ignore a c)
                     (get-list-metrics lss)))
              ($if (>? c 0)
                   ($let ((cycle  (aux1 c (list-tail lss a) ())))
                      ($cond ((pair? cycle)
                                (set-last! cycle cycle)))
                      (aux1 a lss cycle))
                   (aux1 (- a 1) lss (list-ref lss (- a 1))))))))

($define! list-neighbors
   ($lambda (ls)

      ($define! aux
         ($lambda (ls n) ; get n sets of neighbors from ls
            ($if (>? n 0)
                 (cons (list (car ls) (cadr ls))
                       (aux (cdr ls) (- n 1)))
                 ())))

      ($let (((p #ignore a c)  (get-list-metrics ls)))
         ($if (=? c 0)
              (aux ls (- a 1))
              ($let ((ls  (aux ls p)))
                 (encycle! ls a c)
                 ls)))))

($define! filter
   ($lambda (accept? ls)
      (apply append
             (map ($lambda (x)
                     ($if (apply accept? (list x))
                          (list x)
                          ()))
                  ls))))

($define! assoc
   ($lambda (object alist)
      ($let ((alist  (filter ($lambda (record)
                                (equal? object (car record)))
                             alist)))
         ($if (null? alist)
              ()
              (car alist)))))

($define! member?
   ($lambda (object ls)
      (apply or?
             (map ($lambda (x) (equal? object x))
                  ls))))

($define! finite-list?
   ($lambda args
      (apply and?
             (map ($lambda (x)
                     ($let (((#ignore n . #ignore)
                               (get-list-metrics x)))
                        (>? n 0)))
                  args))))

($define! countable-list?
   ($lambda args
      (apply and?
             (map ($lambda (x)
                     ($let (((#ignore n #ignore c)
                             (get-list-metrics x)))
                        ($or? (>? c 0)
                              (>? n 0))))
                  args))))

                  (display ($quote countable-list-loaded))

($define! reduce
   ($let ()

      ($define! reduce-acyclic
         ($lambda (ls bin id)
            ($cond ((null? ls)        id)
                   ((null? (cdr ls))  (car ls))
                   (#t
                      (bin (car ls)
                           (reduce-acyclic (cdr ls) bin id))))))

      ($define! reduce-n
         ($lambda (ls bin n)
            ($if (=? n 1)
                 (car ls)
                 (bin (car ls)
                      (reduce-n (cdr ls) bin (- n 1))))))

      (wrap ($vau (ls bin id . opt) env

         ($define! fixenv
            ($lambda (appv)
               ($lambda x (apply appv x env))))

         ($define! bin (fixenv bin))

         ($let (((p n a c)  (get-list-metrics ls)))
            ($if (=? c 0)
                 (reduce-acyclic ls bin id)
                 ($sequence
                    ($define! (pre in post) (map fixenv opt))
                    ($define! reduced-cycle
                       (post (reduce-n (map pre (list-tail ls a))
                                       in
                                       c)))
                    ($if (=? a 0)
                         reduced-cycle
                         (bin (reduce-n ls bin a)
                              reduced-cycle)))))))))

($define! append!
   ($lambda lss

      ($define! set-last!
         ($lambda (ls tail)
            ($let ((next  (cdr ls)))
               ($if (pair? next)
                    (set-last! next tail)
                    (set-cdr! ls tail)))))

      (map ($lambda (x) (apply set-last! x))
           (list-neighbors (filter ($lambda (x)
                                      (not? (null? x)))
                                   lss)))
      #inert))

($define! assq
   ($lambda (object alist)
      ($let ((alist (filter ($lambda (record)
                               (eq? object (car record)))
                            alist)))
         ($if (null? alist)
              ()
              (car alist)))))

($define! memq?
   ($lambda (object ls)
      (apply or?
             (map ($lambda (x) (eq? object x))
                  ls))))

($define! eq?
   ($let ((old-eq?  eq?))
      ($lambda x
         ($if ($and? (pair? x) (pair? (cdr x)) (null? (cddr x)))
              (apply old-eq? x)
              (apply and?
                     (map ($lambda (x) (apply old-eq? x))
                          (list-neighbors x)))))))
($define! equal?
   ($let ((old-equal?  equal?))
      ($lambda x
         ($if ($and? (pair? x) (pair? (cdr x)) (null? (cddr x)))
              (apply old-equal? x)
              (apply and?
                     (map ($lambda (x) (apply old-equal? x))
                          (list-neighbors x)))))))

; uses continuation, not supported and not needed here
;($define! $binds?
;   ($vau (exp . ss) dynamic
;      (guard-dynamic-extent
;         ()
;         ($lambda ()
;                  ($let ((env  (eval exp dynamic)))
;                     (map ($lambda (sym) (eval sym env))
;                          ss))
;                  #t)
;         (list (list error-continuation
;                     ($lambda (%ignore divert)
;                        (apply divert #f)))))))

($define! get-current-environment (wrap ($vau () e e)))

($define! make-kernel-standard-environment
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
                         (eval exp
                               env))
                   (map cadr bindings))
            env)))

($define! $let-safe
   ($vau (bindings . body) env
      (eval (list* $let-redirect
                   (make-kernel-standard-environment)
                   bindings
                   body)
             env)))

($define! $remote-eval
   ($vau (o e) d
      (eval o (eval e d))))

($define! $bindings->environment
   ($vau bindings denv
      (eval (list $let-redirect
                  (make-environment)
                  bindings
                  (list get-current-environment))
            denv)))

($define! $set!
   ($vau (exp1 formals exp2) env
      (eval (list $define! formals
                  (list (unwrap eval) exp2 env))
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
            (eval exp env))))

($define! for-each
   (wrap ($vau x env
            (apply map x env)
            %inert)))

($provide! (promise? memoize $lazy force)

   ($define! (encapsulate promise? decapsulate)
             (make-encapsulation-type))

   ($define! memoize
      ($lambda (value)
         (encapsulate (list (cons value ())))))

   ($define! $lazy
      ($vau (exp) env
         (encapsulate (list (cons exp env)))))

   ($define! force
      ($lambda (x)
         ($if (not? (promise? x))
              x
              (force-promise (decapsulate x)))))

   ($define! force-promise
      ($lambda (x)
         ($let ((((object . env))  x))
            ($if (not? (environment? env))
                 object
                 (handle-promise-result x (eval object env))))))

   ($define! handle-promise-result
      ($lambda (x y)
         ($cond ((null? (cdar x))    ; check for earlier result
                   (caar x))
                ((not? (promise? y))
                   (set-car! (car x) y)       ;
                   (set-cdr! (car x) ())      ; memoize
                   y)
                (#t
                   (set-car! x (car (decapsulate y)))  ; iterate
                   (force-promise x))))))

($define! abs ($lambda (x) ($if (<? x 0) (* x -1) x)))

($define! min
   ($lambda x

      ($define! aux
         ($lambda (count result . x)
            ($if (<=? count 0)
                 result
                 ($sequence
                    ($if (<? (car x) result)
                         ($define! result ($if (inexact? result)
                                               (* (car x) 1.0)
                                               (car x)))
                         ($if (inexact? (car x))
                              ($define! result (* result 1.0))
                              #inert))
                    (apply aux (list* (- count 1) result (cdr x)))))))

      (apply aux (list* (car (get-list-metrics x))
                        #e+infinity
                        x))))

($define! apply-continuation
    ($lambda (c o)
        (apply (continuation->applicative c) o)))

($define! guard-dynamic-extent
   (wrap ($vau (entry-guards combiner exit-guards) env

      ($let ((local  (get-current-environment)))
         ($let/cc  bypass
            ($set! local bypass bypass)
            (apply-continuation
               (car ($let/cc  cont
                       ($set! local enter-through-bypass
                          (continuation->applicative cont))
                       (list bypass)))
               #inert)))

      ($let/cc  cont
         (enter-through-bypass
            (extend-continuation
               (guard-continuation
                  (cons (list bypass ($lambda (v . #ignore) v))
                        entry-guards)
                  cont
                  exit-guards)
               ($lambda #ignore
                  (apply combiner () env))))))))


($define! $let/cc
   ($vau (symbol . body) env
      (eval (list call/cc (list* $lambda (list symbol) body))
            env)))




            (display ($quote library-loaded))"/* +

            "($define! add1 ($lambda (x) (+ x 1)))" +
            "" +
            "($define! error ($lambda (x) (apply-continuation error-continuation x)))"*/;
        }
    }
}

