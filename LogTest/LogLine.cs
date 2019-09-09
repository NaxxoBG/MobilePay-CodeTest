namespace LogTest
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the diff. loggers (filelogger, consolelogger etc.) will operate on. The LineText() method will be called to get the text (formatted) to log
    /// </summary>
    public class LogLine
    {
        #region Private Fields

        #endregion

        #region Constructors

        public LogLine()
        {
            Text = "";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return a formatted line
        /// </summary>
        /// <returns></returns>
        public string LineText()
        {
            StringBuilder sb = new StringBuilder();

            if (Text.Length > 0)
            {
                sb.Append(Text);
                sb.Append(". ");
            }

            sb.Append(CreateLineText());

            return sb.ToString();
        }

        private string CreateLineText()
        {
            return "";
        }


        #endregion

        #region Properties

        /// <summary>
        /// The text to be display in logline
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Timestamp is initialized when the log is added. Th
        /// </summary>
        public DateTime Timestamp { get; set; }
  

        #endregion
    }
}