using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.mobilenet.mercframework;

namespace it.mintlab.mobilenet.mercframework.test
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
        }

        [StateBinding(state = STATE_A)]
        public void stateA()
        {
            Console.WriteLine("State A");
            State = STATE_B;
        }

        [StateBinding(state = STATE_B)]
        public void stateB()
        {
            Console.WriteLine("State B");
            State = STATE_B;
        }

    }
}