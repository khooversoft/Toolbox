// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public static class CacheObjectExtensions
    {
        public static CacheObject<T> ToCacheObject<T>(this T value, TimeSpan lifeTime, TimeSpan? refresh = null)
            where T : class
        {
            return new CacheObject<T>(lifeTime, refresh)
                .Set(value);
        }
    }
}
