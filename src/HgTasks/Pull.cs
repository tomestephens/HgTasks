using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    /// <summary>
    /// Perform an hg pull set source repo or use 
    /// </summary>
    [TaskName("hg_pull")]
    public class Pull : Task
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

        private bool withUpdate = false;
        /// <summary>
        /// Update to tip after pull
        /// </summary>
        [TaskAttribute("update", Required = false)]
        public bool WithUpdate
        {
            get { return withUpdate; }
            set { withUpdate = value; }
        }

        private string Command
        {
            get
            {
                return string.Format("pull {1} {2}", Source, (WithUpdate ? "--update" : ""));
            }
        }

        protected override void ExecuteTask()
        {
            if (!HgDefaults.IsRepo(Repository))
            {
                Log(Level.Error, "Repository not found.");
                return;
            }

            Log(Level.Info, "Pulling latest for {0}...", Repository.Name);
            HgProcess process = HgProcess.Run(Command, Repository.FullName, FailOnError);

            if (WithUpdate && process.StandardOutput.Contains("no changes found"))
                Log(Level.Info, "No changes found, update not done.");

            if (Verbose)
                Log(Level.Debug, process.StandardOutput);
        }
    }
}
