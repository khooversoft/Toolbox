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
    public class LogFileWriter : ILogFileWriter
    {
        private readonly Serializer<SimpleBinaryWriter<OutputStream>> _serializer;
        private FileStream _file;
        private OutputStream _outputStream;
        private SimpleBinaryWriter<OutputStream> _simpleBinaryWriter;

        public LogFileWriter(string folderPath)
        {
            Verify.IsNotEmpty(nameof(folderPath), folderPath);

            FolderPath = FolderPath;
            _serializer = new Serializer<SimpleBinaryWriter<OutputStream>>(typeof(EventDataRecord));
        }

        public string FolderPath { get; }

        public string LogFileName { get; }

        public Task Open()
        {
            Verify.Assert(_file == null, "Log file already opened");

            Directory.CreateDirectory(FolderPath);
            _file = new FileStream(Path.Combine(FolderPath, $"Log_{Guid.NewGuid().ToString()}.txt"), FileMode.Create);
            _outputStream = new OutputStream(_file);
            _simpleBinaryWriter = new SimpleBinaryWriter<OutputStream>(_outputStream);

            return Task.FromResult(0);
        }

        public void Dispose()
        {
            OutputStream outputStream = Interlocked.Exchange(ref _outputStream, null);
            outputStream?.Flush();

            FileStream writer = Interlocked.Exchange(ref _file, null);
            writer?.Dispose();
        }

        public Task Save(IEnumerable<EventData> eventDataItems)
        {
            Verify.IsNotNull(nameof(eventDataItems), eventDataItems);
            Verify.IsNotNull(nameof(_file), _file);

            if (!eventDataItems.Any())
            {
                return Task.FromResult(0);
            }

            foreach(var item in eventDataItems)
            {
                EventDataRecord record = item.ConvertTo();
                _serializer.Serialize(record, _simpleBinaryWriter);
            }

            return Task.FromResult(0);
        }
    }
}
