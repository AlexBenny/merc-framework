using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace it.mintlab.mobilenet.mercframework
{
    public interface IMercView
    {
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param id="messageContent">Message to delivery</param>
        void deliveryFormMessage(Message msg);

        /// <summary>
        /// Set the merc's form
        /// </summary>
        /// <param id="cmd">form</param>
        void setForm(Form form);
    }
}
