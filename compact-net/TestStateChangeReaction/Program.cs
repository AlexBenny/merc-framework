using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace it.mintlab.mobilenet.mercframework.test
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
