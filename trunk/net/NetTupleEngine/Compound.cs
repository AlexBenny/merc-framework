using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    public class Compound : Term, IRefObject
    {
        private string rootValue;
        private List<Term> leafValues;
        private object refObject;
        private bool withRefObject;

        public Compound(string rootValue, List<Term> leafValues)
        {
            this.rootValue = rootValue;
            this.leafValues = leafValues;
            this.withRefObject = false;
        }

        public void setRefObject(object obj)
        {
            refObject = obj;
            withRefObject = true;
        }

        public override string ToString()
        {
            string leafs = "";
            foreach (Term leaf in leafValues)
            {
                leafs += leaf.ToString() + ",";
            }
            return rootValue + "(" + leafs.Remove(leafs.Length - 1) + ")";
        }

        #region Term Membri di

        internal override bool unify(Term t, List<Var> vars1, List<Var> vars2)
        {
            if (t is Compound)
            {
                Compound tc = (Compound)t;
                if (this.rootValue.Equals(tc.rootValue) &&
                    this.leafValues.Count == tc.leafValues.Count)
                {
                    for (int i = 0; i < leafValues.Count; i++)
                    {
                        if (!this.leafValues[i].unify(tc.leafValues[i], vars1, vars2)) return false;
                    }
                    return true;
                }
            }
            if (t is Var)
            {
                return t.unify(this, vars2, vars1);
            }
            return false;
        }

        public override object getRefObject()
        {
            if (withRefObject)
                return refObject;
            else
                if (rootValue.StartsWith("'"))
                    return rootValue.Replace("'", "");
                else if (rootValue.StartsWith("\""))
                    return rootValue.Replace("\"", "");
                else
                    return rootValue;
        }

        #endregion
    }
}
