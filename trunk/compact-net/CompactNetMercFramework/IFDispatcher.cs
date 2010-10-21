using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.mobilenet.mercframework
{
    /// <summary>
    /// This interface show Merc Node api for the Framework
    /// </summary>
    internal interface IFDispatcher
    {
        /// <summary>
        /// Give message to Merc Node
        /// </summary>
        /// <param id="message">message</param>
        void letMessage(Message message);

        /// <summary>
        /// Kill the Merc
        /// </summary>
        void killMerc();

        /// <summary>
        /// Return Merc State
        /// </summary>
        /// <returns></returns>
        bool isMercAlive();

        
    }

}
