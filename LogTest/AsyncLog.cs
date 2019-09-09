using System.Linq;

namespace LogTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Linq;

    public class AsyncLog : ILog
    {
        private FileHandler _handler;
        private List<LogLine> _lines = new List<LogLine>();
        private bool _exit;
        private bool _quitWithFlush;
        

        public AsyncLog()
        {
            _handler = new FileHandler();
            new Thread(MainLoop).Start();
        }

        private void MainLoop()
        {
            while (!_exit)
            {
                var f = 0;
                var handled = new List<LogLine>();

                foreach (LogLine logLine in _lines.ToList())
                {
                    f++;

                    if (f > 5 || _exit && !_quitWithFlush)
                        continue;

                    handled.Add(logLine);
                    _handler.WriteToLog(logLine);
                }

                handled.ForEach(t => _lines.Remove(t));

                if (_quitWithFlush && _lines.Count == 0)
                {
                    _exit = true;
                    _handler.CloseWriter();
                }
                Thread.Sleep(50);
            }
            Console.WriteLine("DONE WITH LOGGING");
        }

        public void StopWithoutFlush()
        {
            _handler.CloseWriter();
            Console.WriteLine($"Logger stopped without flushing! {_lines.Count} skipped.");
            _exit = true;
        }

        public void StopWithFlush()
        {
            _quitWithFlush = true;
        }

        public void Write(string text)
        {
            _lines.Add(new LogLine() { Text = text, Timestamp = DateTime.Now });
        }

        public ILog NewInstance()
        {
            return new AsyncLog();
        }
    }
}