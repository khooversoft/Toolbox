// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Khooversoft.Toolbox
{
    public class ProcessStopped : IStateItem
    {
        private readonly Tag _tag;

        public ProcessStopped(string exeName, bool ignoreError)
        {
            Verify.IsNotEmpty(nameof(exeName), exeName);

            Name = exeName;
            IgnoreError = ignoreError;
            _tag = new Tag(nameof(ProcessStopped)).With(Name);
        }

        public string Name { get; }

        public bool IgnoreError { get; }

        public Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            var list = Process.GetProcessesByName(Name);
            foreach (var item in list)
            {
                if (context.CancellationToken.IsCancellationRequested)
                {
                    return Task.FromResult(false);
                }

                context.EventLog.Info(context, $"Killing process {item.ProcessName}");
                item.Kill();
            }

            return Task.FromResult(true);
        }

        public Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);

            var list = Process.GetProcessesByName(Name);
            bool result = list.Length == 0;
            context.EventLog.Info(context, $"Test result: {result}");

            return Task.FromResult(result);
        }
    }
}
