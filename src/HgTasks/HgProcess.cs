using System;
using System.Diagnostics;

namespace HgTasks
{
    /// <summary>
    /// A really simple helper class to handle running the process
    /// </summary>
    internal class HgProcess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string StandardOutput { get; private set; }
        public string StandardError { get; private set; }
        public int ExitCode { get; private set; }

        public static HgProcess Run(string arguments, string workingDirectory, bool failOnError)
        {
            log.DebugFormat("Running in {0}.{1}Command: hg {2}", workingDirectory, Environment.NewLine, arguments);

            HgProcess process = new HgProcess();

            Process p = new Process();
            p.StartInfo.FileName = "hg";
            p.StartInfo.Arguments = arguments;
            p.StartInfo.WorkingDirectory = workingDirectory;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            using (p)
            {
                p.Start();
                process.StandardOutput = p.StandardOutput.ReadToEnd();
                process.StandardError = p.StandardError.ReadToEnd();
                p.WaitForExit();
                process.ExitCode = p.ExitCode;
            }

            if (!string.IsNullOrEmpty(process.StandardError))
            {
                string error = string.Format("Error during hg command: {0}{1}Errors:{2}", arguments, Environment.NewLine, process.StandardError);

                if (failOnError)
                    throw new Exception(error);

                log.Error(error);
            }

            return process;
        }
    }
}
