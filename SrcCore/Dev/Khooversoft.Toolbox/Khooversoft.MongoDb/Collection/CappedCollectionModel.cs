// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Khooversoft.MongoDb
{
    public class CappedCollectionModel : CollectionModel
    {
        public int MaxNumberOfDocuments { get; set; }

        public long MaxSizeInBytes { get; set; }

        public override bool IsValid()
        {
            if (!base.IsValid())
            {
                return false;
            }

            return MaxNumberOfDocuments > 0 &&
                MaxSizeInBytes > 0;
        }
    }
}
