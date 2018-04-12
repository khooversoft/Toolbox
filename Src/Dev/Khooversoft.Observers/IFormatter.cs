// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.Observers
{
    public interface IFormatter<TIn, TOut>
    {
        /// <summary>
        /// Format an object, transform from TIn to TOut
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        TOut Format(TIn obj);
    }
}
