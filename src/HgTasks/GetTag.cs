using System;
using System.IO;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace HgTasks
{
    [TaskName("hg_get_tag")]
    public class GetTag : Task
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

            string tag = "";
            string[] lines = process.StandardOutput.Split('\n');

            foreach (string line in lines)
            {
                if (line.Contains("changeset:"))
                {
                    string[] chunks = line.Split(':');
                    tag = chunks[2];
                }
            }

            Project.Properties["HgTag"] = tag;

            Log(Level.Debug, "Set HgTag property to {0}.", tag);
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
