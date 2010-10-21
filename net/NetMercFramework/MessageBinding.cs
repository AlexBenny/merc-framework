using System;
using System.Collections.Generic;
using System.Text;

namespace it.mintlab.desktopnet.mercframework
{

    [AttributeUsage(AttributeTargets.Method)]
    public class MessageBinding : Attribute
    {
        public string message;
        public MessageContent.Category category = MessageContent.Category.COMMAND;

        private string[] _states = new string[1] { StateBinding.DEFAULT_STATE };
        private string[] _senders = new string[0] { };
        public string senderParam;


        public string state
        {
            get { return _states[0]; }
            set { _states[0] = StateBinding.STATE_PREFIX + value; }
        }

        public string[] states
        {
            get { return _states; }
            set
            {
                _states = new string[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    _states[i] = StateBinding.STATE_PREFIX + value[i];
                }
            }
        }


        public string sender
        {
            get { return _senders[0]; }
            set { _senders = new string[1] { value }; }
        }

        public string[] senders
        {
            get { return _senders; }
            set { _senders = value; }
        }


    }

}
