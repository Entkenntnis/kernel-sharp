using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Collections.Generic;

namespace Kernel
{
    public class ParseException : Exception
    {
        public ParseException(string message) : base(message){ }
    }

    public abstract class KObject
    {
        private static string _data;
        private static int _dataIndex;

        public static KObject Parse(string tokens)
        {
            _data = tokens;
            _dataIndex = 0;
            KObject ret = buildExpr();
            if (null == ret)
                throw new ParseException("Empty token!");
            return ret;
        }

        public static List<KObject> ParseAll(string tokens)
        {
            _data = tokens;
            _dataIndex = 0;
            var output = new List<KObject>();
            KObject cur = buildExpr();
            while (cur != null)
            {
                output.Add(cur);
                cur = buildExpr();
            }
            return output;
        }

        private static KObject buildExpr()
        {
            return buildExpr(nextToken());
        }

        private static KObject buildExpr(object next)
        {
            if (next is KObject)
                return (KObject)next;
            else if (next is char)
            {
                // list handling
                char nextc = (char)next;
                if (nextc != '(')
                    throw new ParseException("Unbalanced paranthesis!");
                
                object secondnext = nextToken();
                if (secondnext == null)
                    return null;
                if (secondnext is char && ((char)secondnext) == ')')
                    return new KNil();

                KObject secondnextobj;
                if (secondnext is char && ((char)secondnext) == '(')
                    secondnextobj = buildExpr(secondnext);
                else
                    secondnextobj = secondnext as KObject;

                KPair listTail = new KPair(secondnextobj, null, true);
                KPair listHead = listTail;

                while (true)
                {
                    secondnext = nextToken();
                    if (secondnext == null)
                        throw new ParseException("Open Parens not closed!");
                    if (secondnext is char && (char)secondnext == ')')
                        break;
                    if (secondnext is char && (char)secondnext == '.')
                    {
                        object thirdnext = nextToken();
                        listTail.SetCdr(buildExpr(thirdnext));
                        object forthnext = nextToken();
                        if (forthnext is char && (char)forthnext == ')')
                            break;
                        else
                            throw new ParseException("Dotted list unbalanced!");
                    }
                    KPair newPair = new KPair(buildExpr(secondnext), null, true);
                    listTail.SetCdr(newPair);
                    listTail = newPair;
                }
                if (listTail.Cdr == null)
                    listTail.SetCdr(new KNil());
                
                return KPair.CopyEsImmutable(listHead);
            }
            else
                return null;
        }

        private static object nextChar()
        {
            if (_dataIndex == _data.Length)
                return null;
            char t = _data[_dataIndex];
            _dataIndex++;
            if (t != '\\')
                return t;
            else if (_dataIndex + 1 == _data.Length)
                return t;
            else
            {
                _dataIndex++;
                return _data.Substring(_dataIndex - 2, 2);
            }
        }

        private static char unescape(string x)
        {
            char c = x[1];
            if (c == 'n')
                return '\n';
            if (c == 't')
                return '\t';
            if (c == 'r')
                return '\r';
            if (c == '"')
                return '"';
            if (c == '\\')
                return '\\';
            else
                throw new ParseException("Unknown escape char: " + c.ToString());
        }

        private static object nextToken()
        {
            object thisT;

            // skip whitespace
            while (true)
            {
                thisT = nextChar();
                if (null == thisT)
                    return null;
                if (thisT is char && char.IsWhiteSpace((char)thisT))
                    continue;
                else if (thisT is char && (char)thisT == ';')
                {
                    while (thisT is char && (char)thisT != '\n')
                        thisT = nextChar();
                }
                else
                    break;
            }

            // parse string
            if (thisT is char && (char)thisT == '"')
            {
                StringBuilder sb = new StringBuilder();
                object nextC;
                while (true)
                {
                    nextC = nextChar();
                    if (null == nextC)
                        throw new ParseException("Unbalanced string!");
                    if (nextC is char && (char)nextC  == '"')
                        break;
                    if (nextC is string)
                        sb.Append(unescape((string)nextC));
                    else
                        sb.Append(((char)nextC).ToString());
                }
                return new KString(sb.ToString());
            }

            // unparsed
            if (thisT is char)
            {
                char c = (char)thisT;
                if (c == '(' || c == ')' || c == '.')
                    return c;
            }

            // get joined characters
            StringBuilder joined = new StringBuilder();
            object thatT = thisT;
            while (true)
            {
                if (null == thatT)
                    break;
                else if (thatT is char)
                {
                    char c = (char)thatT;
                    if (c == '(' || c == ')' || char.IsWhiteSpace(c) || c == ';')
                    {
                        _dataIndex--;
                        break;
                    }
                    else
                        joined.Append(((char)thatT).ToString());
                }
                else
                {
                    joined.Append(unescape((string)thatT));
                }
                thatT = nextChar();
            }
            string tok = joined.ToString().ToLower(CultureInfo.InvariantCulture);

            // check hashtag token
            if (tok.StartsWith("#"))
            {
                string tag = tok.Substring(1);
                if (tag.Equals("t"))
                    return new KBoolean(true);
                else if (tag.Equals("f"))
                    return new KBoolean(false);
                else if (tag.Equals("inert"))
                    return new KInert();
                else if (tag.Equals("ignore"))
                    return new KIgnore();
                else if (tag.Equals("e-infinity"))
                    return new KDouble(Double.NegativeInfinity);
                else if (tag.Equals("e+infinity"))
                    return new KDouble(Double.PositiveInfinity);
            }

            // try long number
            long x;
            if (long.TryParse(tok, out x))
                return new KInteger(x);

            // then try double
            double d;
            if (!tok.Contains("Infinity") &&  double.TryParse(tok,NumberStyles.Float, CultureInfo.InvariantCulture, out d))
                return new KDouble(d);

            // well, so it must be a symbol
            return new KSymbol(tok);
        }

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

    // object definitions

    public class KBoolean : KObject
    {
        public bool Value
        {
            get;
            private set;
        }
        public KBoolean (bool value)
        {
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            return Value ? "#t" : "#f";
        }
    }

    public class KSymbol : KObject
    {
        public string Value
        {
            get;
            private set;
        }
        public KSymbol (string value)
        {
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            return Value;
        }
    }

    public class KPair : KObject
    {
        private static KObject copyEs(KObject x, bool mutable, Dictionary<KObject, KObject> visited)
        {
            if (!(x is KPair))
                return x;
            KPair n = new KPair(null, null, true);
            if (visited.ContainsKey(x))
                return visited[x];
            else
                visited.Add(x, n);
            KPair p = x as KPair;
            KObject car = p.Car is KPair ? copyEs(p.Car, mutable, visited) : p.Car;
            KObject cdr = p.Cdr is KPair ? copyEs(p.Cdr, mutable, visited) : p.Cdr;
            n.SetCar(car);
            n.SetCdr(cdr);
            if (!mutable)
                n.MakeImmutable();
            return n;
        }
        public static KObject CopyEs(KObject x)
        {
            return copyEs(x, true, new Dictionary<KObject, KObject>());
        }
        public static KObject CopyEsImmutable(KObject x)
        {
            return copyEs(x, false, new Dictionary<KObject, KObject>());
        }
        // move this to the evaluator! it is only used there
        public static KObject Map(Func<KObject, KObject> f, KObject lst)
        {
            KNil nil = new KNil();
            KPair newlst = null;
            KPair head = newlst;
            List<KObject> visited = new List<KObject>();
            while (true)
            {
                if (lst is KNil)
                {
                    if (null == head)
                        return nil;
                    else
                        return head;
                }
                else if (lst is KPair && visited.Contains(lst))
                {
                    newlst.SetCdr(lst);
                    return newlst;
                }
                else if (lst is KPair)
                {
                    KPair orig = lst as KPair;
                    visited.Add(orig);
                    if (null == newlst)
                        head = newlst = new KPair(f(orig.Car), nil, true);
                    else
                    {
                        KPair newElement = new KPair(f(orig.Car), nil, true);
                        newlst.SetCdr(newElement);
                        newlst = newElement;
                    }
                    lst = orig.Cdr;
                }
                else
                    throw new RuntimeException("Improper list passed to map");
            }                
        }
        public static void Foreach(Action<KObject> f, KObject lst)
        {
            List<KObject> visited = new List<KObject>();
            while (!(lst is KNil))
            {
                if (lst is KPair)
                {
                    KPair cur = lst as KPair;
                    if (visited.Contains(cur))
                        return;
                    else
                        visited.Add(cur);
                    f(cur.Car);
                    lst = cur.Cdr;
                }
                else
                    throw new RuntimeException("Improper list passed to foreach");
            }
            return;
        }
        public static int Length(KObject lst)
        {
            if (lst is KNil)
                return 0;
            List<KObject> visited = new List<KObject>();
            int length = 0;
            if (lst is KPair)
            {
                KPair cur = lst as KPair;
                visited.Add(cur);
                length++;
                while (true)
                {
                    if (cur.Cdr is KNil)
                        return length;
                    else if (cur.Cdr is KPair)
                    {
                        cur = cur.Cdr as KPair;
                        if (visited.Contains(cur))
                            return -1;
                        else
                            visited.Add(cur);
                        length++;
                    }
                    else
                        return -1;
                }
            }
            else
                return -1;
        }
        public KObject Car
        {
            get;
            private set;
        }
        public KObject Cdr
        {
            get;
            private set;
        }
        public bool Mutable
        {
            get;
            private set;
        }
        public KPair (KObject car, KObject cdr, bool mut)
        {
            Car = car;
            Cdr = cdr;
            Mutable = mut;
        }
        public void SetCar (KObject car)
        {
            if (Mutable)
                Car = car;
            else
                throw new Exception("Internal: Attempt to mutate immutable list!");
        }
        public void SetCdr (KObject cdr)
        {
            if (Mutable)
                Cdr = cdr;
            else
                throw new Exception("Internal: Attempt to mutate immutable list!");
        }
        public void MakeImmutable()
        {
            this.Mutable = false;
        }
        public override string Print(bool quoteStrings)
        {
            return Print(quoteStrings, new List<KObject>());
        }
        public override string Print(bool quoteStrings, List<KObject> visited)
        {
            if (visited.Contains(this))
                return "#sref";
            visited.Add(this);
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            KPair cur = this;
            while (true)
            {
                var subvis = new List<KObject>();
                subvis.AddRange(visited);
                sb.Append(cur.Car.Print(quoteStrings, subvis));
                if (cur.Cdr is KNil)
                {
                    sb.Append(")");
                    break;
                }
                else
                {
                    if (cur.Cdr is KPair)
                    {
                        cur = (KPair)cur.Cdr;
                        sb.Append(" ");
                    }
                    else
                    {
                        sb.Append(" . ");
                        sb.Append(cur.Cdr.Print(quoteStrings, visited));
                        sb.Append(")");
                        break;
                    }
                }
                if (visited.Contains(cur))
                {
                    sb.Append(". #sref)");
                    return sb.ToString();
                }
                else
                    visited.Add(cur);
            }
            return sb.ToString();
        }
    }

    public class KNil : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "()";
        }
    }

    public class KIgnore : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "#ignore";
        }
    }

    public class KInert : KObject
    {
        public override string Print(bool quoteStrings)
        {
            return "#inert";
        }
    }

    public class KString : KObject
    {
        public string Value
        {
            get;
            private set;
        }
        public KString(string val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            if (quoteStrings)
                return ToLiteral(Value);
            else
                return Value;
        }
        private static string ToLiteral(string input)
        {
            return "\"" + input.Replace("\n", "\\n").Replace("\"", "\\\"").Replace("\t", "\\t").Replace("\r", "\\r") + "\"";
        }
    }

    public class KInteger : KObject
    {
        public long Value
        {
            get;
            private set;
        }
        public KInteger(long val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            return Value.ToString();
        }
    }

    public class KDouble : KObject
    {
        public double Value
        {
            get;
            private set;
        }

        public KDouble(double val)
        {
            Value = val;
        }
        public override string Print(bool quoteStrings)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    public abstract class KCombiner : KObject
    {
        public abstract RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont);
    }

    public class KApplicative : KCombiner
    {
        public KCombiner Combiner
        {
            get;
            private set;
        }
        public KApplicative(KCombiner op)
        {
            Combiner = op;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<applicative:" + Combiner.Print(quoteStrings) + ">";
        }
        public override RecursionResult<KObject> Combine(KObject args, KEnvironment env, Continuation<KObject> cont)
        {
            return Combiner.Combine(args, env, cont);
        }
    }

    public class KOperative : KCombiner
    {
        private KObject Formals;
        private KObject EFormal;
        private KObject Expr;
        private KEnvironment staticenv;
        public KOperative()
        {
        }
        public KOperative(KObject formals, KObject eformal, KObject expr, KEnvironment env)
        {
            Formals = KPair.CopyEsImmutable(formals);
            EFormal = eformal;
            Expr = KPair.CopyEsImmutable(expr);
            if (!(eformal is KIgnore || eformal is KSymbol))
                throw new RuntimeException("Can't construct operative, type mismatch of eformal");
            var lst = CheckFormalTree(Formals);
            if (eformal is KSymbol)
            {
                KSymbol s = eformal as KSymbol;
                if (lst.Contains(s.Value))
                    throw new RuntimeException("Distinct eformal needed");
            }
            if (!(env is KEnvironment))
                throw new RuntimeException("Operative: not an environment!");
            else
                staticenv = env;
        }
        protected List<string> CheckFormalTree(KObject formaltree)
        {
            var lst = new List<string>();
            if (formaltree is KNil || formaltree is KIgnore)
                return lst;
            else if (formaltree is KSymbol)
            {
                lst.Add(((KSymbol)formaltree).Value);
                return lst;
            }
            else if (formaltree is KPair)
            {
                KPair p = formaltree as KPair;
                lst.AddRange(CheckFormalTree(p.Car));
                lst.AddRange(CheckFormalTree(p.Cdr));
                return lst;
            }
            else
                throw new RuntimeException("Invalid formal tree");
        }
        public override string Print(bool quoteStrings)
        {
            return "#<operative" + (null == Expr ? " p:" + this.GetType().ToString() : "") + ">";
        }
        protected void BindFormalTree(KObject formal, KObject vals, KEnvironment env)
        {
            if (formal is KSymbol)
            {
                env.Bind(((KSymbol)formal).Value, vals);
            }
            else if (formal is KIgnore)
                return;
            else if (formal is KNil && vals is KNil)
                return;
            else if (formal is KPair && vals is KPair)
            {
                KPair f = formal as KPair;
                KPair v = vals as KPair;
                BindFormalTree(f.Car, v.Car, env);
                BindFormalTree(f.Cdr, v.Cdr, env);
            }
            else
                throw new RuntimeException("Can't bind formal tree!");
        }
        public override RecursionResult<KObject> Combine(KObject operands, KEnvironment env, Continuation<KObject> cont)
        {
            if (null == Expr)
                return CPS.Error("Primitive without implementation!", cont);
            KEnvironment local = new KEnvironment(staticenv);
            if (!(EFormal is KIgnore))
                local.Bind(((KSymbol)EFormal).Value, env);
            BindFormalTree(Formals, operands, local);
            return CPS.Next<KObject>(() => Evaluator.rceval(Expr, local, cont), cont);
        }
        protected RecursionResult<KObject> Return(KObject obj, Continuation<KObject> cont)
        {
            return CPS.Return(obj, cont);
        }
        protected RecursionResult<KObject> ReturnBool(bool b, Continuation<KObject> cont)
        {
            return Return(new KBoolean(b), cont);
        }
        protected string CheckParameter(KObject p, int len, string name)
        {
            int actLen = KPair.Length(p);
            if (actLen != len)
            {
                return name + ": mismatching number of ops, expected " + len + ", got " + actLen;
            }
            return null;
        }
        protected KObject First(KObject p)
        {
            return ((KPair)p).Car;
        }
        protected KObject Second(KObject p)
        {
            return ((KPair)((KPair)p).Cdr).Car;
        }
        protected KObject Third(KObject p)
        {
            return (((KPair)((KPair)p).Cdr).Cdr as KPair).Car;
        }
        protected RecursionResult<KObject> Predicate(KObject args, string name, Type t, Continuation<KObject> cont)
        {
            int len = KPair.Length(args);
            if (len == -1)
                return CPS.Error(name + ": parameter is not a list", cont);
            else
            {
                bool result = true;
                try
                {
                KPair.Foreach(x =>
                    {
                        if (!x.GetType().Equals(t))
                            result = false;
                    }, args);
                }
                catch (Exception e)
                {
                    return CPS.Error(e.Message, cont);
                }
                return ReturnBool(result, cont);
            }
        }
    }

    public class KEnvironment : KObject
    {
        private static KEnvironment GroundEnv = null;
        public static KEnvironment GetGroundEnv()
        {
            if (null == GroundEnv)
            {
                GroundEnv = new KEnvironment(null);
                addp("boolean?", new PBoolean());
                addp("equal?", new PEqual());
                addp("eq?", new PEq());
                addp("equal?", new PEqual());
                addp("symbol?", new PSymbol());
                addp("inert?", new PInert());
                addp("pair?", new PPair());
                addp("null?", new PNull());
                addp("environment?", new PEnvironment());
                addp("ignore?", new PIgnore());
                addp("operative?", new POperative());
                addp("applicative?", new PApplicative());
                addp("cons", new PCons());
                addp("eval", new PEval());
                addp("make-environment", new PMakeEnvironment());
                addp("set-car!", new PSetCar());
                addp("set-cdr!", new PSetCdr());
                addp("copy-es-immutable", new PCopyEsImmutable());
                addp("copy-es", new PCopyEs());
                addp("wrap", new PWrap());
                addp("unwrap", new PUnwrap());
                GroundEnv.Bind("$if", new PIf());
                GroundEnv.Bind("$define!", new PDefine());
                GroundEnv.Bind("$vau", new PVau());
                addp("+", new PAdd());
                addp("-", new PSub());
                addp("*", new PMult());
                addp("/", new PDiv());
                addp("number?", new PNumber());
                addp("integer?", new PInteger());
                addp("=?", new PNEq());
                addp("<?", new PNless());
                addp(">?", new PNgreater());
                addp("inexact?", new PInexact());
                addp("write", new PWrite());
                addp("display", new PDisplay());
                addp("make-encapsulation-type", new PEncap());
                addp("continuation?", new PContinuation());
                addp("call/cc", new PCallCC());
                addp("continuation->applicative", new PContinuationToApplicative());
                addp("extend-continuation", new PExtendContinuatione());
                GroundEnv.Bind("root-continuation", new KContinuation(CPS.RootContinuation<KObject>()));
                var errorC = CPS.RootContinuation<KObject>();
                errorC.isError = true;
                GroundEnv.Bind("error-continuation", new KContinuation(errorC));
                addp("guard-continuation", new PGuardContinuation());
                addp("load", new PLoad());

                try
                {
                    var lst = Library.getLibrary();
                    var objs = KPair.ParseAll(lst);
                    foreach (KObject item in objs)
                    {
                        Evaluator.Eval(item, GroundEnv);
                    }
                }
                catch (ParseException e)
                {
                    Console.WriteLine("Library couldn't be loaded: " + e.Message + "\n");
                }
            }
            return GroundEnv;
        }
        private static void addp(string name, KOperative op)
        {
            GroundEnv.Bind(name, new KApplicative(op));
        }
        Dictionary<string, KObject> table = new Dictionary<string, KObject>();
        List<KEnvironment> parents = new List<KEnvironment>();
        public KEnvironment()
        {
        }
        public KEnvironment(KEnvironment parent)
        {
            parents.Add(parent);
        }
        public void AddParent(KEnvironment env)
        {
            parents.Add(env);
        }
        public KObject Lookup(string name)
        {
            KObject obj;
            bool found = table.TryGetValue(name, out obj);
            if (!found)
            {
                foreach (var env in parents)
                {
                    if (null == env)
                        break;
                    obj = env.Lookup(name);
                    if (obj is KObject)
                    {
                        found = true;
                        break;
                    }
                }
            }
            return found ? obj : null;
        }
        public void Bind(string name, KObject value)
        {
            if (table.ContainsKey(name))
                table[name] = value;
            else
                table.Add(name, value);
        }
        public override string Print(bool quoteStrings)
        {
            return "#<environment>";
        }
    }

    public class KEncapsulation : KObject
    {
        public int Id
        {
            get;
            private set;
        }
        public KObject Value
        {
            get;
            private set;
        }
        public KEncapsulation(int id, KObject value)
        {
            this.Id = id;
            Value = value;
        }
        public override string Print(bool quoteStrings)
        {
            return "#<encapsulation:" + Id + ">";
        }
    }

    public struct KGuard
    {
        public KContinuation Selector;
        public KApplicative Interceptor;
    }

    public class KContinuation : KObject
    {
        public Continuation<KObject> Value
        {
            get;
            private set;
        }
        private Continuation<KObject> fixStructure(Continuation<KObject> c)
        {
            if (c.Parent == null)
                return c; // base continuation without state
            else
            {
                var thatsnew = new Continuation<KObject>(c.NextStep, fixStructure(c.Parent), c.Context);
                thatsnew.isError = c.isError;
                if (c._Placeholder != null)
                {
                    var remaing = new LinkedList<KObject>(c._RemainingObjs);
                    var pars = new LinkedList<KObject>(c._Pairs);
                    thatsnew._Pairs = pars;
                    thatsnew._RemainingObjs = remaing;
                    thatsnew._Placeholder = c._Placeholder;
                }
                return thatsnew;
            }
        }
        public KContinuation(Continuation<KObject> cont)
        {
            Value = fixStructure(cont);
        }
        public override string Print(bool quoteStrings)
        {
            return "#<continuation>";
        }
    }
}