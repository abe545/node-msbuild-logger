using System;
using System.Reflection;
using Microsoft.Build.Framework;

[assembly: AssemblyVersion("0.0.1.0")]
[assembly: AssemblyTitle("Strider MSBuild Logger")]

namespace Strider.MsBuild
{
    public class Logger : ILogger
    {
        private const string errorPrefix = "\u001b[31;1m";
        private const string warnPrefix  = "\u001b[31;3m";
        private const string clearFormat = "\u001b[0m";

        public LoggerVerbosity Verbosity  { get; set; }
        public string          Parameters { get; set; }

        public void Initialize(IEventSource eventSource)
        {           
            eventSource.ProjectStarted  += new ProjectStartedEventHandler (OnProjectStarted);
            eventSource.ProjectFinished += new ProjectFinishedEventHandler(OnProjectFinished);
            eventSource.WarningRaised   += new BuildWarningEventHandler   (OnWarningRaised);
            eventSource.ErrorRaised     += new BuildErrorEventHandler     (OnErrorRaised);
        }

        public void Shutdown()
        {
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            Console.WriteLine("Building " + e.Message);
        }

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void OnWarningRaised(object sender, BuildWarningEventArgs e)
        {
            Console.WriteLine("{0}{1}({2},{3}): warning {4}: {5}{6}", warnPrefix, e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, clearFormat);
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            Console.Error.WriteLine("{0}{1}({2},{3}): error {4}: {5}{6}", errorPrefix, e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message, clearFormat);
        }
    }
}
