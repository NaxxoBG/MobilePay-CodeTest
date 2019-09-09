using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Engine.Internal;

namespace LogUsers
{
    using System.Threading;

    using LogTest;

    class Program
    {
        static void Main(string[] args)
        {
            ILog logger = new AsyncLogV2();
            RunLogger(logger);
            Console.ReadLine();
        }

        private static void RunLogger(ILog logger)
        {

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILog logger2 = logger.NewInstance();

            for (int i = 50; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i);
                Thread.Sleep(20);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();
        }
    }
}
