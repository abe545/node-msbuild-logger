using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Build.Framework;

[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyTitle("Strider MSBuild Logger")]

namespace Strider.MsBuild
{
    public class Logger : ILogger
    {
        private const string errorPrefix   = "\u001b[31;1m";
        private const string successPrefix = "\u001b[32;1m";
        private const string warnPrefix    = "\u001b[33;1m";
        private const string infoPrefix    = "\u001b[36;1m";
        private const string verbosePrefix = "\u001b[37m";
        private const string clearFormat   = "\u001b[0m";

        private readonly Queue<string> errors   = new Queue<string>();
        private readonly Queue<string> warnings = new Queue<string>();
        private          DateTime      startTime;

        public LoggerVerbosity Verbosity  { get; set; }
        public string          Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildStarted    += new BuildStartedEventHandler   (OnBuildStarted);
            eventSource.ProjectStarted  += new ProjectStartedEventHandler (OnProjectStarted);
            eventSource.MessageRaised   += new BuildMessageEventHandler   (OnMessageRaised);
            eventSource.WarningRaised   += new BuildWarningEventHandler   (OnWarningRaised);
            eventSource.ErrorRaised     += new BuildErrorEventHandler     (OnErrorRaised);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(OnProjectFinished);
            eventSource.BuildFinished   += new BuildFinishedEventHandler  (OnBuildFinished);
        }

        public void Shutdown()
        {
        }

        private void OnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            startTime = e.Timestamp;
            Console.WriteLine("Build Started {0}", e.Timestamp);
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Console.Write(infoPrefix);
            Console.WriteLine("Building {0}", e.Message);
            Console.Write(clearFormat);
        }

        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (e.Importance == MessageImportance.Normal)
            {
                Console.Write(verbosePrefix);
                Console.WriteLine(e.Message);
                Console.Write(clearFormat);
            }
        }

        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            Console.Write(warnPrefix);
            var warn = string.Format("{0}({1},{2}): warning {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
            Console.WriteLine(warn);
            warnings.Enqueue(warn);
            Console.Write(clearFormat);
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            Console.Error.Write(errorPrefix);
            var err = string.Format("{0}({1},{2}): error {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
            Console.Error.WriteLine(err);
            errors.Enqueue(err);
            Console.Error.Write(clearFormat);
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            if (e.Succeeded)
            {
                Console.Write(successPrefix);
                Console.WriteLine("{0}", e.Message);
                Console.Write(clearFormat);
            }
            else
            {
                Console.Error.Write(errorPrefix);
                Console.Error.WriteLine("{0}", e.Message);
                Console.Error.Write(clearFormat);
            }
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            Console.WriteLine(e.Message);

            foreach(var s in errors)
            {
                Console.Write(errorPrefix);
                Console.WriteLine(s);
                Console.Write(clearFormat);
            }
            foreach (var s in warnings)
            {
                Console.Write(warnPrefix);
                Console.WriteLine(s);
                Console.Write(clearFormat);
            }

            if (warnings.Count > 0)
                Console.Write(warnPrefix);
            Console.WriteLine("{0} Warning(s)", warnings.Count);
            Console.Write(clearFormat);

            if (errors.Count > 0)
                Console.Write(errorPrefix);
            Console.WriteLine("{0} Error(s)", errors.Count);
            Console.Write(clearFormat);

            Console.WriteLine("Time Elapsed {0}", e.Timestamp - startTime);
        }
    }
}
