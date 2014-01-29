using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    /// <summary>
    /// A task to run a set of hg tasks that prepare a repo for use
    /// </summary>
    [TaskName("hg_prepare")]
    public class Prepare : Task
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
            // verify the repo first
            // checking for a pending transaction or corruption
            Verify verify = new Verify()
            {
                DeleteIfUnverified = this.DeleteIfUnverified,
                Repository = this.Repository,
                FailOnError = this.FailOnError,
                Verbose = this.Verbose
            };
            verify.Execute();

            // now clone if needed
            if (!HgDefaults.IsRepo(Repository))
            {
                Clone clone = new Clone()
                {
                    Repository = this.Repository,
                    FailOnError = this.FailOnError,
                    Source = this.Source,
                    Verbose = this.Verbose
                };
                clone.Execute();
            }
            // repo exists so lets pull the latest
            else
            {
                Pull pull = new Pull()
                {
                    Repository = this.Repository,
                    FailOnError = this.FailOnError,
                    Source = this.Source,
                    Verbose = this.Verbose
                };
                pull.Execute();
            }

            // update to the correct branch and rev/tag
            Update update = new Update()
            {
                Repository = this.Repository,
                FailOnError = this.FailOnError,
                Clean = this.Clean,
                Branch = this.Branch,
                Rev = this.Rev,
                Verbose = this.Verbose
            };
            update.Execute();
        }
    }
}
