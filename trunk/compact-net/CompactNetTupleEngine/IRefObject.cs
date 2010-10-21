using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.tupleengine
{
    public interface IRefObject
    {
        void setRefObject(object obj);

        object getRefObject();

    }
}
