using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.tupleengine
{
    public interface IRefObject
    {
        void setRefObject(object obj);
        
        object getRefObject();
    }
}
