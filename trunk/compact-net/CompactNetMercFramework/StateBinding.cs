using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.mercframework
{
    public class StateBinding : Attribute
    {
        public const string DEFAULT_STATE = "DEFAULT";
        public const string STATE_PREFIX = "@";

        public string state;
    }

}
