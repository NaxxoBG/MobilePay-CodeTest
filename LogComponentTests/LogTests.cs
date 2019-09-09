using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using LogTest;
using NUnit.Framework;

namespace LogComponentTests
{
    public class LogTests
    {

        [SetUp]
        public void Setup()
        {
            var files = Directory.GetFiles(@"C:\LogTest");
            foreach (var file in files)
            {
                while (IsFileLocked(new FileInfo(file)))
                {
                    continue;
                }
                File.Delete(file);
            }
        }

        [TearDown]
        public void Dispose()
        {

        }

        [Test]
        public void TestStopWithoutFlush()
        {
            ILog logger = new AsyncLogV2();
            for (int i = 50; i > 0; i--)
            {
                logger.Write("Number with No flush: " + i);
                Thread.Sleep(15);
            }
            logger.StopWithoutFlush();
            string s = File.ReadAllText(Directory.GetFiles(@"C:\LogTest")[0]);
            Console.WriteLine(s);
            Assert.False(s.EndsWith("1."));
        }

        [Test]
        public void TestStopWithFlush()
        {
            ILog logger = new AsyncLogV2();

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i);
                Thread.Sleep(50);
            }

            logger.StopWithFlush();
            var file = Directory.GetFiles(@"C:\LogTest")[0];

            while (IsFileLocked(new FileInfo(file)))
            {
                continue;
            }
            string s = File.ReadAllText(file);
            Console.WriteLine(s.Substring(s.Length-7, 3));
            Assert.True(s.Substring(s.Length - 7, 3).Equals("14."));
        }

        [Test]
        public void TestWrite()
        {
            ILog logger = new AsyncLogV2();
            var res = Guid.NewGuid().ToString();
            logger.Write(res);
            logger.StopWithFlush();
            var dir = new DirectoryInfo(@"C:\LogTest");
            var cFlag = false;
            foreach (var fileInfo in dir.GetFiles())
            {
                while (IsFileLocked(fileInfo))
                {
                    continue;
                }
                if (File.ReadAllText(fileInfo.FullName).Contains(res))
                {
                    cFlag = true;
                }
            }
            Assert.IsTrue(cFlag);
        }

        [Test]
        public void CheckInterpolatedStrings()
        {
            Console.WriteLine($"{"Timestamp".PadRight(25, ' ')} \t {"Data".PadRight(15, ' ')} \t {Environment.NewLine}");
            Console.WriteLine($@"C:\LogTest\Log{DateTime.Now:yyyyMMdd HHmmss fff}.log");
            //Console.WriteLine(new AsyncLogV2().GetType());
        }

        [Test]
        [Timeout(15000)]
        public void TestNewFileAtMidnight()
        {
            var cr = DateTime.Now.Day;
            var curDate = DateTime.Now;

            if (curDate.Hour != 23 && curDate.Minute != 59)
            {
                Console.WriteLine($"Current Time: {curDate:HH:mm:ss tt}. Make sure to set the time to 23:59:50 when running the test");
                Assert.Fail();
            }

            ILog logger = new AsyncLogV2();
            while (true)
            {
                logger.Write($" ----- {DateTime.Now.Day} - {cr} = Answer: {DateTime.Now.Day - cr}");
                logger.Write("Original estimation, != than 0 -> " + (DateTime.Now - curDate).Days);
                Thread.Sleep(500);
                if (DateTime.Now.Day - cr != 0)
                {
                    logger.Write($" ----- {DateTime.Now.Day} - {cr} = Answer: {DateTime.Now.Day - cr}");
                    Thread.Sleep(100);
                    break;
                }
            }
            logger.StopWithoutFlush();
            Assert.IsTrue(new DirectoryInfo(@"C:\LogTest").GetFiles().Length == 2);
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }
    }
}