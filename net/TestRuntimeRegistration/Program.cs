using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace it.mintlab.desktopnet.mercframework.test.runtimeregistration
{
    class Program
    {
        static void Main(string[] args)
        {
            Framework f = new Framework();
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}

