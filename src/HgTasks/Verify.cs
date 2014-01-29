using System;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    /// <summary>
    /// A task to run hg verify
    /// 
    /// You can optionally delete the repository if an error is encountered.
    /// I use this on our continuous integration server where occasionally the
    /// repo becomes corrupted - then I can easily re-clone the repo before continuing.
    /// </summary>
    [TaskName("hg_verify")]
    public class Verify : Task
    {
        private DirectoryInfo repository;
        /// <summary>
        /// Full path to the repository
        /// </summary>
        [TaskAttribute("repo", Required = true)]
        public DirectoryInfo Repository
        {
            get { return repository; }
            set { repository = value; }
        }

        private bool deleteIfUnverified = true;
        /// <summary>
        /// Do you want to delete the repo if there is an error
        /// </summary>
        [TaskAttribute("deleteIfUnverified", Required = false)]
        public bool DeleteIfUnverified
        {
            get { return deleteIfUnverified; }
            set { deleteIfUnverified = value; }
        }

        protected override void ExecuteTask()
        {
            if (!HgDefaults.IsRepo(Repository))
            {
                Log(Level.Info, "Repository not found.");
                return;
            }

            Log(Level.Info, "Running hg verify on {0}.", Repository.Name);

            HgProcess process = HgProcess.Run("verify", Repository.FullName, false);

            if (!string.IsNullOrEmpty(process.StandardError))
            {
                Log(Level.Error, "Repository was not verified. Errors:{0}{1}", Environment.NewLine, process.StandardError);

                if (DeleteIfUnverified)
                {
                    Log(Level.Info, "Deleting repository...");
                    Repository.Delete(true);
                }
            }
            else
            {
                Log(Level.Info, "No errors found.");
            }

            Log(Level.Info, "Completed.");
        }
    }
}
