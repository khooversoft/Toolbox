// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;

namespace Khooversoft.Net
{
    public static class RestClientExtensions
    {
        public static RestClient SetCv(this RestClient self, CorrelationVector cv = null)
        {
            Verify.IsNotNull(nameof(self), self);

            cv = cv ?? new CorrelationVector();

            self.Properties.TryAdd(() => new CvHeader(cv));
            return self;
        }
    }
}
