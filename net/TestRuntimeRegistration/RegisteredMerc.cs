using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.runtimeregistration
{
    public class RegisteredMerc : Merc
    {
        [MessageBinding(message = "anybody")]
        public void m1()
        {
            dispatcher.logInfo("Test Error");
            dispatcher.registerMerc("Registered", "it.mintlab.desktopnet.mercframework.test.RegistrantMerc, TestRuntimeRegistration");
        }

        [MessageBinding(message = "andNow")]
        public void m2()
        {
            dispatcher.logInfo("Test OK");
        }

        [MessageBinding(message = "deregisterYou")]
        public void m3()
        {
            dispatcher.deregisterMerc();
        }

    }
}
