using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    class VarToken : IToken
    {
        private string var;

        public VarToken(string var)
        {
            this.var = var;
        }

        public override string ToString()
        {
            return var;
        }


    }
}
