// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Telemetry
{
    public interface ILogFileReader : IDisposable
    {
        LogFileReader Open();

        IEnumerable<EventData> Read(int recordMaxCount);
    }
}
