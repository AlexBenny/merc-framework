using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.remotereceiver
{
    class ReceiverMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init()
        {
            dispatcher.logInfo("Waiting for friend..");
        }

        [MessageBinding(message = "hello")]
        public void hello()
        {
            dispatcher.deliverMessage("Sender", "question('How are you?')");
        }

        [MessageBinding(message = "response(_msg)")]
        public void response(string msg)
        {
        }

        [MessageBinding(message = "question(_msg)")]
        public void question(string msg)
        {
            dispatcher.deliverMessage("Sender", "response('I do not know')");
        }

        [MessageBinding(message = "bye")]
        public void bye()
        {
            dispatcher.killMerc();
        }
    }
}
