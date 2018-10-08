// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public interface IStateItem
    {
        string Name { get; }

        bool IgnoreError { get; }

        Task<bool> Test(IWorkContext context);

        Task<bool> Set(IWorkContext context);
    }
}
