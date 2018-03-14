using Khooversoft.Toolbox;
using System;
using System.IO;
using System.Threading;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Text file observer, write events to a file
    /// </summary>
    /// <typeparam name="T">type for observer</typeparam>
    public class TextFileObserver<T> : ObserverBase<T>
    {
        private readonly IFormatter<T, string> _formatter;
        protected StreamWriter _outputFile;

        public TextFileObserver(string fileName, IFormatter<T, string> formatter)
        {
            Verify.IsNotEmpty(nameof(fileName), fileName);
            Verify.IsNotNull(nameof(formatter), formatter);

            FileName = fileName;
            _formatter = formatter;
        }

        /// <summary>
        /// File name for text file
        /// </summary>
        public string FileName { get; protected set; }

        /// <summary>
        /// Open text file for output
        /// </summary>
        /// <returns>this</returns>
        public virtual TextFileObserver<T> Open()
        {
            Verify.Assert(_outputFile == null, "Log file must not be opened");

            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            _outputFile = new StreamWriter(FileName, false);
            _outputFile.AutoFlush = true;
            return this;
        }

        /// <summary>
        /// Close the file
        /// </summary>
        protected void Close()
        {
            StreamWriter sw = Interlocked.Exchange(ref _outputFile, null);
            if (sw != null)
            {
                sw.Close();
            }
        }

        protected override void OnCompletedCore()
        {
            _outputFile?.Close();
        }

        protected override void OnErrorCore(Exception error)
        {
            _outputFile?.Close();
        }

        protected override void OnNextCore(T value)
        {
            _outputFile?.WriteLine(_formatter.Format(value));
        }
    }
}
