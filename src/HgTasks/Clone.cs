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
    [TaskName("hg_clone")]
    public class Clone : Task
    {
        private DirectoryInfo repository;
        /// <summary>
        /// Path to local repo
        /// </summary>
        [TaskAttribute("repo", Required = true)]
        public DirectoryInfo Repository
        {
            get { return repository; }
            set { repository = value; }
        }

        private string source = string.Empty;
        /// <summary>
        /// Url to Source repo
        /// Uses default as set in hgrc by default
        /// </summary>
        [TaskAttribute("source", Required = false)]
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        private bool removeIfExists = false;
        /// <summary>
        /// Do you want to remove the repo if it exists in order to have a fresh clone?
        /// </summary>
        [TaskAttribute("removeIfExists", Required = false)]
        public bool RemoveIfExists
        {
            get { return removeIfExists; }
            set { removeIfExists = value; }
        }

        private string Command
        {
            get
            {
                return string.Format("clone {0} {1}", Source, Repository.Name);
            }
        }

        protected override void ExecuteTask()
        {
            if (HgDefaults.IsRepo(Repository))
            {
                Log(Level.Debug, "Repository found.");

                if (RemoveIfExists)
                {
                    Log(Level.Info, "Deleting for re-clone.");
                    Repository.Delete(true);
                }
                else
                {
                    // repo exists - clone is not necessary
                    return;
                }
            }

            Log(Level.Info, "Cloning {0}...", Repository.Name);

            HgProcess process = HgProcess.Run(Command, Repository.Parent.FullName, FailOnError);

            if (Verbose)
                Log(Level.Debug, process.StandardOutput);
            
            Log(Level.Info, "Completed.");
        }
    }
}
