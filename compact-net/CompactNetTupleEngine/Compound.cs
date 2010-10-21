using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public class Compound : Term, IRefObject
    {
        private string rootValue;
        private List<Term> leafValues;
        private object refObject;

        public Compound(string rootValue, List<Term> leafValues)
        {
            this.rootValue = rootValue;
            this.leafValues = leafValues;
        }

        public override string ToString()
        {
            string leaves = "";
            foreach (Term leaf in leafValues)
            {
                leaves += leaf.ToString() + ",";
            }
            return rootValue + "(" + leaves.Remove(leaves.Length - 1,1) + ")";
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

        #endregion


        #region IRefObject Membri di

        public void setRefObject(object obj)
        {
            refObject = obj;
        }

        public override object getRefObject()
        {
            return refObject == null ? rootValue : refObject;
        }

        #endregion
    }
}
