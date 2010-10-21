using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public abstract class Term
    {
        internal abstract bool unify(Term t, List<Var> vars1, List<Var> vars2);

        public abstract object getRefObject();

        public bool unify(Term t, Dictionary<string, Term> varMap)
        {
            List<Var> vars1 = new List<Var>();
            List<Var> vars2 = new List<Var>();
            lock (this)
            {
                bool result = unify(t, vars1, vars2);
                foreach (Var v in vars1)
                {
                    varMap.Add(v.getName(), v.getValue());
                    v.deunify();
                }
                return result;
            }
        }

        public bool match(Term t)
        {
            List<Var> vars1 = new List<Var>();
            List<Var> vars2 = new List<Var>();
            lock (this)
            {
                bool result = unify(t, vars1, vars2);
                foreach (Var v in vars1) v.deunify();
                foreach (Var v in vars2) v.deunify();
                return result;
            }
        }

        public override bool Equals(object obj)
        {
            return this.match((Term)obj);
        }
    }
}
