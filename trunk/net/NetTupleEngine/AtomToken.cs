using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    class AtomToken : IToken
    {
        private string atom;
        private object parameter;
        private bool withParameter;

        public AtomToken(string atom)
        {
            this.atom = atom;
            this.withParameter = false;
            this.parameter = null;
        }

        public void setParameter(object parameter)
        {
            this.parameter = parameter;
            this.withParameter = true;
        }

        public object getParameter()
        {
            return parameter;
        }

        public override string ToString()
        {
            return atom;
        }

        public bool isWithParameter() 
        {
            return this.withParameter;
        }


    }
}
