using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.mercframework
{
    internal interface IMerc
    {
        /// <summary>
        /// Dispatcher Setter
        /// </summary>
        /// <param id="dispatcher"></param>
        void setDispatcher(IMDispatcher dispatcher);

        /// <summary>
        /// Get the state of Merc
        /// </summary>
        /// <returns></returns>
        string getState();

        /// <summary>
        /// Set Binding beetween state and method to call for state changing
        /// </summary>
        void loadStateBinding();
    }
}
