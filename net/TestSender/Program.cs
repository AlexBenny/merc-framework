using System;
using System.Collections.Generic;
using System.Text;
using it.mintlab.desktopnet.mercframework;
using System.Threading;

namespace it.mintlab.desktopnet.mercframework.test.remotesender
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Framework f = new Framework();
            // Loop for debug purpose to see logging
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
