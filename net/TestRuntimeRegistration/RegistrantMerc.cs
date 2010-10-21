using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.runtimeregistration
{
    public class RegistrantMerc : Merc
    {
        [MessageBinding(message = "init")]
        public void init()
        {
            dispatcher.deliverMessage("Registered", "anybody");
            dispatcher.registerMerc("Registered", "it.mintlab.desktopnet.mercframework.test.RegisteredMerc, TestRuntimeRegistration");
            dispatcher.deliverMessage("Registered", "andNow");
            dispatcher.deliverMessage("Registered", "deregisterYou");
            dispatcher.killMerc();
        }
    }
}
