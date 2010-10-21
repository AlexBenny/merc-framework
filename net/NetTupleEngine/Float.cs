using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    public class Float : Term
    {
        private float value;


        public Float(float value)
        {
            this.value = value;
        }

        public float getValue()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }


        #region Term Membri di

        internal override bool unify(Term t, List<Var> vars1, List<Var> vars2)
        {
            if (t is Float)
            {
                float f = ((Float)t).getValue();
                {
                    if (f == this.value) return true;
                }
            }
            if (t is Int)
            {
                int i = ((Int)t).getValue();
                {
                    if (i == this.value) return true;
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
            return value;
        }

        #endregion
    }
}
