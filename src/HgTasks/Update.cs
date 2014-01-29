using System;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    /// <summary>
    /// A task to run hg update
    /// 
    /// Can update to a target branch and rev if the required params are set
    /// Updates to the default branch by default
    /// </summary>
    [TaskName("hg_update")]
    public class Update : Task
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

        private string branch = HgDefaults.Branch;
        /// <summary>
        /// Branch to update to
        /// </summary>
        [TaskAttribute("branch", Required = false)]
        public string Branch
        {
            get { return branch; }
            set { branch = value; }
        }

        private string rev;
        /// <summary>
        /// Changeset or Tag to update to
        /// </summary>
        [TaskAttribute("rev", Required = false)]
        public string Rev
        {
            get { return rev; }
            set { rev = value; }
        }

        private bool clean = true;
        /// <summary>
        /// Clean repo on update?
        /// Defaults to true
        /// </summary>
        [TaskAttribute("clean", Required = false)]
        public bool Clean
        {
            get { return clean; }
            set { clean = value; }
        }

        private string ToBranchCommand
        {
            get
            {
                return string.Format("update {0} {1}", (Clean ? "--clean" : ""), Branch);
            }
        }

        private string ToRevCommand
        {
            get
            {
                return string.Format("update {0} --rev {1}", (Clean ? "--clean" : ""), Rev);
            }
        }

        protected override void ExecuteTask()
        {
            if (!HgDefaults.IsRepo(Repository))
            {
                Error("Repository not found.");
                return;
            }

            // update to tip of selected branch
            Log(Level.Info, "Updating to branch {0}.", Branch);
            HgProcess process = HgProcess.Run(ToBranchCommand, Repository.FullName, FailOnError);

            if (!string.IsNullOrEmpty(Rev))
            {
                Log(Level.Info, "Updating to Rev {0}.", Rev);
                process = HgProcess.Run(ToRevCommand, Repository.FullName, FailOnError);
            }

            if (Verbose)
                Log(Level.Debug, process.StandardOutput);

            Log(Level.Info, "Completed.");
        }

        protected void Error(string message, params object[] args)
        {
            string err = string.Format(message, args);

            if (FailOnError)
                throw new Exception(err);

            Log(Level.Error, err);
        }
    }
}
