using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.remotereceiver
{
    class ReceiverFriendMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init()
        {
        }

        [MessageBinding(message = "hello")]
        public void hello()
        {
        }

        [MessageBinding(message = "response(_msg)")]
        public void response(string msg)
        {
        }

        [MessageBinding(message = "question(_msg)")]
        public void question(string msg)
        {
            dispatcher.deliverMessage("Sender", "response('Looking for a present!')");
        }

        [MessageBinding(message = "bye")]
        public void bye()
        {
            dispatcher.killMerc();
        }
    }
}
