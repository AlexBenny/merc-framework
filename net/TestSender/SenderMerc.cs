using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.remotesender
{
    class SenderMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init() 
        {
            dispatcher.deliverMessage("Receiver", "hello");
        }

        [MessageBinding(message = "hello")]
        public void hello()
        {
        }

        [MessageBinding(message = "question(_msg)")]
        public void question(string msg)
        {
            dispatcher.deliverMessage("Receiver", "response('Not bad..')");
            dispatcher.deliverMessage(Message.BROADCAST_REMOTE, "question('What are you doing, guys?')");
        }

        [MessageBinding(message = "response(_msg)")]
        public void response(string msg)
        {
            dispatcher.deliverMessage(Message.BROADCAST_ALL, "bye");
            dispatcher.killMerc();
        }

    }
}
