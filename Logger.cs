using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Build.Framework;

[assembly: AssemblyVersion("0.2.0.0")]
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

        private readonly Queue<string>   errors   = new Queue<string>();
        private readonly Queue<string>   warnings = new Queue<string>();
        private          DateTime        startTime;
        private          string          currentTarget;
        private          string          parameters;
        private          LoggerVerbosity verbosity;

        public LoggerVerbosity Verbosity 
        {
            get { return verbosity; }
            set { verbosity = value; }
        }
        public string Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildStarted    += new BuildStartedEventHandler   (OnBuildStarted);
            eventSource.ProjectStarted  += new ProjectStartedEventHandler (OnProjectStarted);
            eventSource.TargetStarted   += new TargetStartedEventHandler  (OnTargetStarted);
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
            string targets = e.TargetNames;
            if (string.IsNullOrEmpty(targets))
                targets = "default targets";

            WriteLine(infoPrefix, "Project \"{0}\" ({1}).", e.ProjectFile, targets);
        }

        private void OnTargetStarted(object sender, TargetStartedEventArgs e)
        {
            currentTarget = e.TargetName;
            
        }

        private void OnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (e.Importance == MessageImportance.High)
            {
                WriteLine(infoPrefix, "{0}:", currentTarget);
                WriteLine(verbosePrefix, e.Message);
            }
        }

        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            string warn = string.Format("{0}({1},{2}): warning {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
            warnings.Enqueue(warn);
            WriteLine(warnPrefix, warn);
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            string err = string.Format("{0}({1},{2}): error {3}: {4}", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
            errors.Enqueue(err);
            WriteError(err);
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            if (e.Succeeded)
                WriteLine(successPrefix, "{0}", e.Message);
            else
                WriteError(e.Message);
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine();
            if (errors.Count > 0)
                WriteLine(errorPrefix, e.Message);
            else if (warnings.Count > 0)
                WriteLine(warnPrefix, e.Message);
            else
                WriteLine(successPrefix, e.Message);

            Console.WriteLine();

            foreach (string s in errors)
                WriteLine(errorPrefix, s);
            foreach (string s in warnings)
                WriteLine(warnPrefix, s);
            
            Console.WriteLine();

            if (warnings.Count > 0)
                Console.Write(warnPrefix);
            Console.Write("\t{0} Warning(s)", warnings.Count);
            Console.WriteLine(clearFormat);

            if (errors.Count > 0)
                Console.Write(errorPrefix);
            Console.Write("\t{0} Error(s)", errors.Count);
            Console.WriteLine(clearFormat);

            Console.WriteLine("Time Elapsed {0}", e.Timestamp - startTime);
        }

        private static void WriteLine(string colorCommand, string format, params object[] args)
        {
            Console.Write(colorCommand);
            Console.Write(format, args);
            Console.WriteLine(clearFormat);
        }

        private static void WriteError(string error)
        {
            Console.Error.WriteLine("{0}{1}{2}", errorPrefix, error, clearFormat);
        }
    }
}
