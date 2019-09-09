using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LogTest
{
    internal class FileHandler
    {
        private StreamWriter _writer;
        private int _curDate = DateTime.Now.Day;
        private string LogDirectory { get; set; } = @"C:\LogTest";

        public FileHandler()
        {
            SetupDirectory();
            SetupLogFile();
            _writer.AutoFlush = true;
        }

        private void SetupDirectory()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        private string GenerateFullLogFilePath()
        {
            return $@"{LogDirectory}\Log{DateTime.Now:yyyyMMdd HHmmss fff}.log";
        }

        private void SetupLogFile()
        {
            var fileHeader = $"{"Timestamp".PadRight(25, ' ')}\t{"Data".PadRight(15, ' ')}\t{Environment.NewLine}";
            _writer = File.AppendText(GenerateFullLogFilePath()); 
            _writer.Write(fileHeader);
        }

        public void CloseWriter()
        {
            _writer.Close();
        }

        public void WriteToLog(LogLine logLine)
        {
            var stringBuilder = new StringBuilder();
            try
            {
                if (DateTime.Now.Day - _curDate != 0)
                {
                    _curDate = DateTime.Now.Day;
                    SetupLogFile();
                    stringBuilder.Append(Environment.NewLine);
                    _writer.Write(stringBuilder.ToString());
                    _writer.AutoFlush = true;
                }
                stringBuilder.Append(logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                stringBuilder.Append($"\t{logLine.LineText()}\t{Environment.NewLine}");
                _writer.Write(stringBuilder.ToString());
            }
            catch (IOException e)
            {
                Console.WriteLine("I/O error with accessing file." + e);
            }
            catch (ObjectDisposedException)
            {
                Console.WriteLine("Stopped without flushing, past entries will not be logged.");
            }
        }
    }
}
