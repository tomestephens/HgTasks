using System;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    /// <summary>
    /// Get the Changeset of the tip of the repo and put it
    /// in to the "changeset" property
    /// </summary>
    [TaskName("hg_get_changeset")]
    public class GetChangeset : Task
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

        private string Command
        {
            get
            {
                return "log -r tip";
            }
        }

        protected override void ExecuteTask()
        {
            if (!HgDefaults.IsRepo(Repository))
            {
                Error("Repository not found.");
                return;
            }

            Log(Level.Info, "Running hg log on {0}.", Repository.Name);

            HgProcess process = HgProcess.Run(Command, Repository.FullName, FailOnError);

            string cs = "";
            string[] lines = process.StandardOutput.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains("changeset:"))
                {
                    string[] chunks = line.Split(':');
                    cs = chunks[2];
                }
            }

            Project.Properties["changeset"] = cs;

            Log(Level.Debug, "Set changeset property to {0}.", cs);
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
