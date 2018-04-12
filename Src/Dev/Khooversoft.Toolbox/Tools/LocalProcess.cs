// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Khooversoft.Toolbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Tools
{
    public class LocalProcess
    {
        public LocalProcess()
        {
            CreateNoWindow = true;
            RemoveBlankLine = true;
        }

        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }
        public int? SuccessExitCode { get; set; }
        public bool RemoveBlankLine { get; set; }
        public string File { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }

        public int ExitCode { get; private set; }
        public Action<string> StandardOutputAction { get; set; }
        public Action<string> StandardErrorAction { get; set; }
        public Process Process { get; private set; }

        public LocalProcess SetUseShellExecute(bool value) { UseShellExecute = value; return this; }

        public LocalProcess SetCreateNoWindow(bool value) { CreateNoWindow = value; return this; }

        public LocalProcess SetSuccessExitCode(int value) { SuccessExitCode = value; return this; }

        public LocalProcess SetRemoveBlankLine(bool value) { RemoveBlankLine = value; return this; }

        public LocalProcess SetFile(string value) { File = value.AssertNotEmpty(); return this; }

        public LocalProcess SetArguments(string value) { Arguments = value; return this; }

        public LocalProcess SetWorkingDirectory(string value) { WorkingDirectory = value; return this; }

        public LocalProcess SetOutputAction(Action<string> action)
        {
            StandardOutputAction = action;
            StandardErrorAction = action;
            return this;
        }

        public static LocalProcess Run(string file, string arguments, string workingDirectory = null, CancellationToken? token = null)
        {
            return new LocalProcess()
                .SetFile(file)
                .SetArguments(arguments)
                .SetWorkingDirectory(workingDirectory)
                .Start()
                .WaitAndMonitor(token);
        }

        public Task RunInBackground(CancellationToken token)
        {
            Start();
            var task = Task.Run(() => WaitAndMonitor(token));
            return task;
        }

        public LocalProcess Start()
        {
            Verify.IsNotEmpty(nameof(File), File);

            Process = new Process();
            Process.StartInfo.FileName = File;
            Process.StartInfo.Arguments = Arguments.IsNotEmpty() ? Arguments : null;
            Process.StartInfo.WorkingDirectory = WorkingDirectory.IsNotEmpty() ? WorkingDirectory : null;

            Process.StartInfo.UseShellExecute = UseShellExecute;
            Process.StartInfo.CreateNoWindow = CreateNoWindow;

            if (UseShellExecute == false)
            {
                Process.StartInfo.RedirectStandardOutput = true;
                Process.StartInfo.RedirectStandardError = true;
            }

            // Start process
            Verify.Assert<InvalidOperationException>(Process.Start(), "Could not start process");

            return this;
        }

        public LocalProcess WaitAndMonitor(CancellationToken? token = null)
        {
            // Wait for process to finish if we are not capturing output
            // Get output
            var tasks = new List<Task>();
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            try
            {
                tasks.Add(Task.Run(() => OutputStream(Process.StandardOutput, true, tokenSource.Token)));
                tasks.Add(Task.Run(() => OutputStream(Process.StandardError, false, tokenSource.Token)));

                if (token == null)
                {
                    Process.WaitForExit();
                }
                else
                {
                    while (!Process.WaitForExit(100) && token?.IsCancellationRequested == false) ;
                    if (!Process.HasExited)
                    {
                        Process.Kill();
                    }
                }
            }
            finally
            {
                tokenSource.Cancel();
            }

            // Wait for tasks to exit
            Task.WaitAll(tasks.ToArray(), 1000);

            // Log status
            ExitCode = Process.ExitCode;

            // Close process
            Process.Close();

            if (SuccessExitCode.HasValue == true && ExitCode != SuccessExitCode)
            {
                throw new ArgumentException(string.Format("Exit code: {0} does not match required exit code {1}", ExitCode, SuccessExitCode));
            }

            return this;
        }

        private void OutputStream(StreamReader reader, bool standard, CancellationToken token)
        {
            Verify.IsNotNull(nameof(reader), reader);

            while (token.IsCancellationRequested)
            {
                string str = reader.ReadLine();
                if (str == null) { continue; }
                if (RemoveBlankLine == true && str.Trim().IsNotEmpty() == true) { continue; }

                // Save output
                if (standard == true)
                {
                    StandardOutputAction?.Invoke(str);
                }
                else
                {
                    StandardErrorAction?.Invoke(str);
                }
            }
        }
    }
}
