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

namespace Khooversoft.Telemetry
{
    public class LogFileReader : IDisposable
    {
        private readonly Deserializer<FastBinaryReader<InputStream>> _serializer;
        private FileStream _file;
        private InputStream _inputStream;
        private FastBinaryReader<InputStream> _binaryReader;

        public LogFileReader(string logFileName)
        {
            Verify.IsNotEmpty(nameof(logFileName), logFileName);

            LogFileName = logFileName;
            _serializer = new Deserializer<FastBinaryReader<InputStream>>(typeof(EventDataRecord));
        }

        public string LogFileName { get; }

        public LogFileReader Open()
        {
            Verify.Assert(_file == null, "Log file already opened");
            Verify.Assert(File.Exists(LogFileName), $"{LogFileName} does not exist");

            _file = new FileStream(LogFileName, FileMode.Open);
            _inputStream = new InputStream(_file);
            _binaryReader = new FastBinaryReader<InputStream>(_inputStream);

            return this;
        }

        public void Dispose()
        {
            FileStream reader = Interlocked.Exchange(ref _file, null);
            reader?.Dispose();
        }

        public IEnumerable<EventData> Read(int recordMaxCount)
        {
            Verify.Assert(recordMaxCount > 0, $"{nameof(recordMaxCount)} must be greater then 0");

            var list = new List<EventData>();

            for(int i = 0; i < recordMaxCount; i++)
            {
                Deserialize.Result result = _serializer.TryDeserialize<EventDataRecord>(_binaryReader, out EventDataRecord eventDataRecord);
                if( result != Deserialize.Result.Success)
                {
                    break;
                }

                list.Add(eventDataRecord.ConverTo());
            }

            return list;
        }
    }
}
