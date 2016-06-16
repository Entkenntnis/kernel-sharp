using System;
using System.Collections.Generic;

namespace Kernel
{
    public class KEnvironment : KObject
    {
        
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
            if (table.Count == 0 && parents.Count == 0)
            {
                return "#empty-env";
            } else {
                return "#<environment:" + String.Join(",", new List<string>(table.Keys)) + ">";
            }
        }

        public override bool CompareTo(KObject other)
        {
            return other is KEnvironment && other == this;
        }
    }
}

