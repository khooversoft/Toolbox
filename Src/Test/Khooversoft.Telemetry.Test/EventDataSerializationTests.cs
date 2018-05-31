using Bond;
using Bond.IO.Safe;
using Bond.Protocols;
using FluentAssertions;
using Khooversoft.Telemetry.Schemas;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Khooversoft.Telemetry.Test
{
    [Trait("Category", "Toolbox")]
    public class EventDataSerializationTests
    {
        [Fact]
        public void TestEventDataSerialization()
        {
            const string cvText = "CV:1234";

            var variables = new KeyValuePair<string, object>[]
            {
                new KeyValuePair<string, object>("string", "string.Value"),
                new KeyValuePair<string, object>("int", 33),
                new KeyValuePair<string, object>("long", 101),
                new KeyValuePair<string, object>("date", DateTime.Now),
            };

            var data = Utility.CreateData(1);
            EventDataRecord record = data.ConvertTo();

            var output = new OutputBuffer();
            var writer = new SimpleBinaryWriter<OutputBuffer>(output);
            var writeSerializer = new Serializer<SimpleBinaryWriter<OutputBuffer>>(typeof(EventDataRecord));

            writeSerializer.Serialize(record, writer);

            var input = new InputBuffer(output.Data);
            var reader = new SimpleBinaryReader<InputBuffer>(input);
            var readSerializer = new Deserializer<SimpleBinaryReader<InputBuffer>>(typeof(EventDataRecord));

            EventDataRecord resultRecord = readSerializer.Deserialize<EventDataRecord>(reader);
            EventData result = resultRecord.ConverTo();

            Utility.VerifyEventData(data, result);
        }

        [Fact]
        public void TestMultiple()
        {
            const int count = 10;

            var eventDataItems = new List<EventData>();
            var rnd = new Random();

            var output = new OutputBuffer();
            var writer = new SimpleBinaryWriter<OutputBuffer>(output);
            var writeSerializer = new Serializer<SimpleBinaryWriter<OutputBuffer>>(typeof(EventDataRecord));

            foreach (var index in Enumerable.Range(0, count))
            {
                var data = Utility.CreateData(index);
                EventDataRecord record = data.ConvertTo();

                eventDataItems.Add(data);
                writeSerializer.Serialize(record, writer);
            }

            var input = new InputBuffer(output.Data);
            var reader = new SimpleBinaryReader<InputBuffer>(input);
            var readSerializer = new Deserializer<SimpleBinaryReader<InputBuffer>>(typeof(EventDataRecord));

            foreach (var index in Enumerable.Range(0, count))
            {
                EventDataRecord resultRecord = readSerializer.Deserialize<EventDataRecord>(reader);
                EventData result = resultRecord.ConverTo();

                Utility.VerifyEventData(eventDataItems[index], result);
            }

            Action act = () => Deserialize<EventDataRecord>.From(reader);
            act.Should().Throw<EndOfStreamException>();
        }
    }
}
