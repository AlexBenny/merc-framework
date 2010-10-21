using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    class IntToken : IToken
    {
        private int valueInt;

        public IntToken(int number)
        {
            this.valueInt = number;
        }

        public int getValue()
        {
            return valueInt;
        }

        public override string ToString()
        {
            return valueInt.ToString();
        }

    }
}
