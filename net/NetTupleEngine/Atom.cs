using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    public class Atom : Term, IRefObject
    {
        private string value;
        private object refObject;
        private bool withRefObject;

        public Atom(string value)
        {
            this.value = value;
            withRefObject = false;
        }

        public void setRefObject(object obj)
        {
            refObject = obj;
            withRefObject = true;
        }

        public override string ToString()
        {
            return value;
        }

        #region Term Membri di

        internal override bool unify(Term t, List<Var> vars1, List<Var> vars2)
        {
            if (t is Atom)
            {
                Atom ta = (Atom)t;
                if (this.value.Equals(ta.value))
                {
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
            if(withRefObject)
                return refObject;
            else
                if(value.StartsWith("'"))
                    return value.Replace("'","");
                else if(value.StartsWith("\""))
                    return value.Replace("\"","");
                else
                    return value;
        }

        #endregion

    }
}
