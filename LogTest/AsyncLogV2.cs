using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace LogTest
{
    public class AsyncLogV2 : ILog
    {
        private readonly BlockingCollection<LogLine> _messageQueue;
        private FileHandler _handler;
        private Thread _wThread;
        private bool _exit;
        private bool _withFlush;

        public AsyncLogV2()
        {
            _messageQueue = new BlockingCollection<LogLine>();
            StartProcessing();
        }

        private void StartProcessing()
        {
            _handler = new FileHandler();
            _wThread = new Thread(ProcessQueue) 
            {
                IsBackground = true
            };
            _wThread.Start();
        }
    
        private void ProcessQueue()
        {
            while (!_exit)
            {
                if (_withFlush && _messageQueue.Count == 0)
                {
                    _handler.CloseWriter();
                    break;
                }

                if (_messageQueue.Count == 0) continue;

                var msg = _messageQueue.Take();
                _handler.WriteToLog(msg);
                Console.WriteLine(msg.Text);
                Thread.Sleep(30);
            }
        }

        public void StopWithoutFlush()
        {
            _exit = true;
            _handler.CloseWriter();
            _messageQueue.CompleteAdding();
        }

        public void StopWithFlush()
        {
            _messageQueue.CompleteAdding();
            _withFlush = true;
        }

        public void Write(string text)
        {
            _messageQueue.Add(new LogLine() { Text = text, Timestamp = DateTime.Now });
        }

        public ILog NewInstance()
        {
            return new AsyncLogV2();
        }
    }
}
