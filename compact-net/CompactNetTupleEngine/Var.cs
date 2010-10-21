using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public class Var : Term
    {
        private string varName;
        private Term value;
        private bool unified;


        public Var(string varName)
        {
            this.varName = varName;
            this.value = null;
            this.unified = false;
        }

        public string getName()
        {
            return varName;
        }

        public Term getValue()
        {
            if (unified && value is Var)
            {
                return ((Var)value).getValue();
            }
            return value;
        }

        internal void setValue(Term value)
        {
            this.value = value;
            this.unified = true;
        }

        internal void deunify()
        {
            this.value = null;
            this.unified = false;
        }

        public bool isUnified()
        {
            if (!unified) return false;
            if (value is Var) return ((Var)value).isUnified();
            return true;
        }

        public override string ToString()
        {
            return isUnified() ? varName + "=" + getValue().ToString() : varName;
        }



        #region Term Membri di

        internal override bool unify(Term t, List<Var> vars1, List<Var> vars2)
        {
            if (unified)
            {
                return value.unify(t, vars1, vars2);
            }
            else
            {
                setValue(t);
                vars1.Add(this);
                return true;
            }
        }

        #endregion

        public override object getRefObject()
        {
            return isUnified() ? getValue().getRefObject() : null;
        }
    }
}
