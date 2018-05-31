// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry.Test
{
    internal static class Utility
    {
        private static Random _rnd = new Random();

        public static EventData CreateData(int index)
        {
            var variables = new List<KeyValuePair<string, object>>();

            foreach (var indexLoop in Enumerable.Range(0, _rnd.Next(0, 10)))
            {
                int mod = indexLoop % 4;
                switch (mod)
                {
                    case 0:
                        variables.Add(new KeyValuePair<string, object>("string", "string.Value"));
                        break;

                    case 1:
                        variables.Add(new KeyValuePair<string, object>("int", _rnd.Next(-100, 100)));
                        break;

                    case 2:
                        variables.Add(new KeyValuePair<string, object>("long", _rnd.Next(-1000, 1000)));
                        break;

                    case 3:
                        variables.Add(new KeyValuePair<string, object>("date", DateTime.Now));
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return new EventData(
                timestamp: DateTime.UtcNow,
                version: 1,
                eventName: $"Name_{index}",
                eventSourceName: $"SourceName_{index}",
                eventLevel: EventLevel.Informational,
                tag: $"Tag_{index}",
                cv: "CV_{index}",
                properties: variables
                );
        }

        public static void VerifyEventData(EventData source, EventData result)
        {
            source.Should().NotBeNull();
            result.Should().NotBeNull();

            result.Timestamp.Should().Be(source.Timestamp);
            result.Version.Should().Be(source.Version);
            result.EventName.Should().Be(source.EventName);
            result.EventSourceName.Should().Be(source.EventSourceName);
            result.EventLevel.Should().Be(source.EventLevel);
            result.Tag.Should().Be(source.Tag);
            result.Cv.Should().Be(source.Cv);

            if (source.Properties == null && result.Properties == null)
            {
                return;
            }

            source.Properties.Should().NotBeNull();
            result.Properties.Should().NotBeNull();
            source.Properties.Count.Should().Be(result.Properties.Count);

            var sourceList = source.Properties.ToList();
            var resultStatck = new Stack<KeyValuePair<string, object>>(result.Properties);

            sourceList.Count.Should().Be(resultStatck.Count);

            while (resultStatck.Count > 0)
            {
                var item = resultStatck.Pop();

                foreach (var index in Enumerable.Range(0, sourceList.Count))
                {
                    KeyValuePair<string, object> compare = sourceList[index];
                    if (item.Key == compare.Key && item.Value.Equals(compare.Value))
                    {
                        sourceList.RemoveAt(index);
                        break;
                    }
                }
            }

            sourceList.Count.Should().Be(0);
            resultStatck.Count.Should().Be(0);
        }
    }
}
