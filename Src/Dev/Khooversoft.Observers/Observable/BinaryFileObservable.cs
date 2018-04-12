// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.IO;
using System.Threading;

namespace Khooversoft.Observers
{
    public class BinaryFileObservable<T> : ObservableBase<T>, IDisposable
    {
        private const byte _recordStart = 0xFF;
        private const byte _recordEnd = 0xFE;
        private readonly IFormatter<byte[], T> _formatter;
        private readonly bool _autoOnCompleted;
        protected FileStream _inputFile;
        protected BinaryReader _binaryInputFile;

        public BinaryFileObservable(string fileName, IFormatter<byte[], T> formatter, bool autoOnCompleted = true)
        {
            Verify.IsNotEmpty(nameof(fileName), fileName);
            Verify.IsNotNull(nameof(formatter), formatter);

            FileName = fileName;
            _formatter = formatter;
            _autoOnCompleted = autoOnCompleted;
        }

        /// <summary>
        /// File name for text file
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Open file and make sure it has the file markers
        /// </summary>
        /// <returns>this</returns>
        public virtual BinaryFileObservable<T> Open()
        {
            Verify.Assert(_inputFile == null, "Log file must not be opened");

            Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            _inputFile = new FileStream(FileName, FileMode.Open);
            _binaryInputFile = new BinaryReader(_inputFile);

            const int size = 2;
            const string msg = "Bad file format (no begin file marks found)";
            byte[] array = _binaryInputFile.ReadBytes(size);
            Verify.Assert<FormatException>(array.Length== size, msg);
            Verify.Assert<FormatException>(array[0] == _recordStart, msg);
            Verify.Assert<FormatException>(array[0] == _recordStart, msg);

            return this;
        }

        /// <summary>
        /// Read the file and send to observers
        /// </summary>
        /// <returns>this</returns>
        public virtual BinaryFileObservable<T> Read()
        {
            byte[] dataArray;

            while (TryGetNextRecord(out dataArray))
            {
                _subject.OnNext(_formatter.Format(dataArray));
            }

            if (_autoOnCompleted)
            {
                _subject.OnCompleted();
            }

            Close();

            return this;
        }

        /// <summary>
        /// On completed signal
        /// </summary>
        /// <returns>this</returns>
        public virtual BinaryFileObservable<T> OnCompleted()
        {
            _subject.OnCompleted();
            return this;
        }

        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Close the file (idempotent)
        /// </summary>
        protected void Close()
        {
            BinaryReader br = Interlocked.Exchange(ref _binaryInputFile, null);
            br?.Dispose();

            FileStream fs = Interlocked.Exchange(ref _inputFile, null);
            if (fs != null)
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Try read the next record
        /// </summary>
        /// <param name="dataArray">data array to be returned</param>
        /// <returns>true if okay, false if end of file</returns>
        private bool TryGetNextRecord(out byte[] dataArray)
        {
            dataArray = null;

            try
            {
                Verify.Assert<InvalidOperationException>(_binaryInputFile != null, "File must be open");

                if (_binaryInputFile.BaseStream.Position >= _binaryInputFile.BaseStream.Length)
                {
                    return false;
                }

                Verify.Assert<FormatException>(_binaryInputFile.ReadByte() == _recordStart, "Missing begin record mark");

                int length = _binaryInputFile.ReadInt32();
                Verify.Assert<FormatException>(length >= 0, "Record length value is invalid");

                dataArray = new byte[length];
                int readLength = _binaryInputFile.Read(dataArray, 0, length);
                Verify.Assert<FormatException>(readLength == length, $"Reading record failed.  Length required={length}, read length:{readLength}");

                Verify.Assert<FormatException>(_binaryInputFile.ReadByte() == _recordEnd, "Missing begin record mark");
                return true;
            }
            catch (Exception ex)
            {
                Close();
                base._subject.OnError(ex);

                throw;
            }
        }
    }
}
