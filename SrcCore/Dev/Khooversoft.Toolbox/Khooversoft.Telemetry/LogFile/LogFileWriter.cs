// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Bond;
using Bond.IO.Unsafe;
using Bond.Protocols;
using Khooversoft.Telemetry.Schemas;
using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Khooversoft.Telemetry
{
    public class LogFileWriter : IEventDataWriter
    {
        private readonly Serializer<FastBinaryWriter<OutputStream>> _serializer;
        private FileStream _file;
        private OutputStream _outputStream;
        private FastBinaryWriter<OutputStream> _binaryWriter;

        public LogFileWriter(string folderPath)
        {
            Verify.IsNotEmpty(nameof(folderPath), folderPath);

            LogFileName = Path.Combine(folderPath, $"Log_{Guid.NewGuid().ToString()}.txt");
            _serializer = new Serializer<FastBinaryWriter<OutputStream>>(typeof(EventDataRecord));
        }

        public string LogFileName { get; }

        public IEventDataWriter Open()
        {
            Verify.Assert(_file == null, "Log file already opened");

            Directory.CreateDirectory(Path.GetDirectoryName(LogFileName));
            _file = new FileStream(LogFileName, FileMode.Create);
            _outputStream = new OutputStream(_file);
            _binaryWriter = new FastBinaryWriter<OutputStream>(_outputStream);

            return this;
        }

        public IEventDataWriter Write(EventData eventDataItem)
        {
            Verify.IsNotNull(nameof(eventDataItem), eventDataItem);
            Verify.Assert(_file != null, "Not open");

            EventDataRecord record = eventDataItem.ConvertTo();
            _serializer.Serialize(record, _binaryWriter);

            return this;
        }

        public void Dispose()
        {
            OutputStream outputStream = Interlocked.Exchange(ref _outputStream, null);
            outputStream?.Flush();

            FileStream writer = Interlocked.Exchange(ref _file, null);
            writer?.Dispose();
        }
    }
}
