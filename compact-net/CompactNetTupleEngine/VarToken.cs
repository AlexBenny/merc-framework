using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
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
