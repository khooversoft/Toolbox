// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;

namespace Khooversoft.Observers
{
    public class ConsoleObserver<T> : ObserverBase<T>
    {
        private readonly IFormatter<T, string> _formatter;

        public ConsoleObserver(IFormatter<T, string> formatter)
        {
            Verify.IsNotNull(nameof(formatter), formatter);

            _formatter = formatter;
        }

        protected override void OnNextCore(T value)
        {
            Console.WriteLine(_formatter.Format(value));
        }
    }
}
