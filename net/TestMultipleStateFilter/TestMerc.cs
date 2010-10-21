using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;

namespace it.mintlab.desktopnet.mercframework.test.multiplestatefilter
{
    class TestMerc : Merc
    {
        const string STATE_A = "STATE_A";
        const string STATE_B = "STATE_B";
        const string STATE_C = "STATE_C";


        [MessageBinding(message = "init")]
        public void init()
        {
            State = STATE_A;
            dispatcher.deliverMessage("TestTupleWithUpperCase");
        }

        [MessageBinding(message = "TestTupleWithUpperCase", states = new string[] { STATE_A, STATE_B })]
        public void f1()
        {
            if (State == STATE_A)
            {
                State = STATE_B;
                dispatcher.deliverMessage("TestTupleWithUpperCase");
            }
            if (State == STATE_B)
            {
                State = STATE_C;
                dispatcher.deliverMessage("testFilterSender");
            }
        }

        [MessageBinding(message = "testFilterSender", senderParam = "_sender", sender = "Test")]
        public void f2(string sender)
        {
            Console.WriteLine("OK! " + sender);
        }

    }
}
