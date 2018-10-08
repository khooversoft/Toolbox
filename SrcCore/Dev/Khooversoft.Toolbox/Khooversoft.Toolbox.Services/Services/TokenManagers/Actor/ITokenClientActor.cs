// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox.Actor;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox.Services
{
    public interface ITokenClientActor : IActor
    {
        Task SetConfiguration(IWorkContext context, IClientTokenManagerConfiguration clientTokenManagerConfiguration);

        Task<string> GetApiKey(IWorkContext context);
    }
}
