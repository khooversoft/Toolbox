// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.IO;
using System.Threading;

namespace Khooversoft.Observers
{
    /// <summary>
    /// Write binary records to file.  This is a forward only observer where there is not rewind
    /// or replay.  Just open, write, close.
    /// 
    /// Beginning of file has 2 X 0xFF start record marks
    /// 
    /// Each record is
    ///   0xFF (begin marker
    ///   Size (int)
    ///   Payload (Array of bytes)
    ///   0xFE (end marker)
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public class BinaryFileObserver<T> : ObserverBase<T>
    {
        private const byte _recordStart = 0xFF;
        private const byte _recordEnd = 0xFE;
        private readonly IFormatter<T, byte[]> _formatter;
        protected FileStream _outputFile;
        protected BinaryWriter _binaryOutputFile;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">file name to write to</param>
        /// <param name="formatter">formatter from type to bytes</param>
        /// <param name="enableRolloverManager">enable </param>
        public BinaryFileObserver(string fileName, IFormatter<T, byte[]> formatter)
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
        public virtual BinaryFileObserver<T> Open()
        {
            Verify.Assert(_outputFile == null, "Log file must not be opened");

            Directory.CreateDirectory(Path.GetDirectoryName(this.FileName));

            _outputFile = new FileStream(FileName, FileMode.Create);
            _binaryOutputFile = new BinaryWriter(_outputFile);

            _binaryOutputFile.Write(_recordStart);
            _binaryOutputFile.Write(_recordStart);
            return this;
        }

        /// <summary>
        /// Close the file
        /// </summary>
        protected void Close()
        {
            BinaryWriter bw = Interlocked.Exchange(ref _binaryOutputFile, null);
            bw?.Flush();
            bw?.Dispose();

            FileStream fs = Interlocked.Exchange(ref _outputFile, null);
            if (fs != null)
            {
                fs.Close();
            }
        }

        protected override void OnCompletedCore()
        {
            this.Close();
        }

        protected override void OnErrorCore(Exception error)
        {
            this.Close();
        }

        protected override void OnNextCore(T value)
        {
            Verify.Assert<InvalidOperationException>(_binaryOutputFile != null, "File is not open");

            byte[] array = _formatter.Format(value);

            _binaryOutputFile.Write(_recordStart);
            _binaryOutputFile.Write(array.Length);
            _binaryOutputFile.Write(array, 0, array.Length);
            _binaryOutputFile.Write(_recordEnd);
        }
    }
}
