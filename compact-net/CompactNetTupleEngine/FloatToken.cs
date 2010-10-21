using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    class FloatToken : IToken
    {
        private float valueFloat;


        public FloatToken(float number)
        {
            this.valueFloat = number;
        }

        public float getValue()
        {
            return valueFloat;
        }

        public override string ToString()
        {
            return valueFloat.ToString();
        }
    }
}
