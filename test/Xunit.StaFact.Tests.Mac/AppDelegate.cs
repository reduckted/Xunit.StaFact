// Copyright (c) Andrew Arnott. All rights reserved.
// Licensed under the Ms-PL license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Xunit.Runner.InProc.SystemConsole;

namespace Xunit.StaFact.Tests.Mac;

[Register("AppDelegate")]
public class AppDelegate : NSApplicationDelegate
{
    private readonly List<string> unitTestDriverArguments = new List<string>();

    public AppDelegate()
    {
        var args = NSProcessInfo.ProcessInfo.Arguments;
        for (int i = 1; i < args.Length; i++)
        {
            this.unitTestDriverArguments.Add(args[i]);
        }
    }

    public override void DidFinishLaunching(NSNotification notification)
    {
        // We cannot run tests in parallel since our tests depend
        // on ordered execution on the main thread.
        this.unitTestDriverArguments.Add("-parallel");
        this.unitTestDriverArguments.Add("none");

        var args = this.unitTestDriverArguments.ToArray();

        ThreadPool.QueueUserWorkItem(o =>
        {
            var exitCode = ConsoleRunner.Run(args).GetAwaiter().GetResult();
            _exit(exitCode);
        });
    }

    [DllImport(ObjCRuntime.Constants.libcLibrary)]
#pragma warning disable SA1300 // Element should begin with upper-case letter
    private static extern void _exit(int exitCode);
#pragma warning restore SA1300 // Element should begin with upper-case letter
}
