using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public class ParsingException : Exception
    {

        public ParsingException(string text)
           : base(text)
        {
        }

        
    }
}
