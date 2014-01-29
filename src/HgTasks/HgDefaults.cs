using System.IO;

namespace HgTasks
{
    internal class HgDefaults
    {
        public const string Branch = "default";

        public static bool IsRepo(DirectoryInfo dir)
        {
            return Directory.Exists(Path.Combine(dir.FullName, ".hg"));
        }
    }
}
