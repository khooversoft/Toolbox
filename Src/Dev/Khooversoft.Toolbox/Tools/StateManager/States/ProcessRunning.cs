// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace Khooversoft.Toolbox
{
    public class ProcessRunning : IStateItem
    {
        private readonly Tag _tag;
        private Task _hostTask;
        private IWorkContext _workContext;

        public ProcessRunning(string exeFullFile, bool ignoreError, string arguments = null)
        {
            Verify.IsNotEmpty(nameof(exeFullFile), exeFullFile);
            Verify.Assert(arguments == null || arguments.IsNotEmpty(), $"{nameof(arguments)} is invalid");

            ExeFullFile = exeFullFile;
            Name = Path.GetFileName(ExeFullFile);
            IgnoreError = ignoreError;
            Arguments = arguments;

            _tag = new Tag(nameof(ProcessRunning)).With(Name);
        }

        public string Name { get; }

        public bool IgnoreError { get; }

        public string ExeFullFile { get; }

        public string Arguments { get; }

        public Task RunningTask { get { return _hostTask; } }

        public bool ProcessStarted { get; private set; }

        public Task<bool> Set(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = _workContext = context.WithTag(_tag);
            ProcessStarted = false;

            AssertFileExists(context);

            Task t = new LocalProcess()
                .SetFile(ExeFullFile)
                .SetWorkingDirectory(Path.GetDirectoryName(ExeFullFile))
                .SetArguments(Arguments)
                .SetOutputAction(OutputToLogging)
                .RunInBackground(context.CancellationToken);

            Interlocked.Exchange(ref _hostTask, t);
            ProcessStarted = true;

            return Task.FromResult(true);
        }

        public Task<bool> Test(IWorkContext context)
        {
            Verify.IsNotNull(nameof(context), context);
            context = context.WithTag(_tag);
            ProcessStarted = false;

            var list = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Name));
            bool result = list.Length != 0;
            ToolboxEventSource.Log.Info(context, $"Test result: {result}");

            if (result)
            {
                return Task.FromResult(result);
            }

            AssertFileExists(context);
            return Task.FromResult(false);
        }

        private bool AssertFileExists(IWorkContext context)
        {
            context = context.WithTag(_tag);

            if (!File.Exists(ExeFullFile))
            {
                string msg = $"Cannot find file {ExeFullFile} to run, current directory={Directory.GetCurrentDirectory()}";
                ToolboxEventSource.Log.Error(context, msg);
                throw new FileNotFoundException(msg, ExeFullFile);
            }

            return true;
        }

        private void OutputToLogging(string value)
        {
            if (value.IsNotEmpty())
            {
                ToolboxEventSource.Log.Verbose(_workContext, $"Process {Name} logging: {value}");
            }
        }
    }
}
