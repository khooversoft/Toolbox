﻿// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
{
    public interface IServerTokenManager
    {
        Task<string> CreateAutorizationToken(IWorkContext context, string requestToken);
    }
}
