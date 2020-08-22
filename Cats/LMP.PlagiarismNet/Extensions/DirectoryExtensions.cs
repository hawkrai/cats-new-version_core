using System.Collections.Generic;
using System.IO;

namespace LMP.PlagiarismNet.Extensions
{
    public static class CustomDirectory
    {
        public static List<string> GetFiles(string path, string searchPattern,
            SearchOption option)
        {
            var patterns = searchPattern.Split('|');
            var files = new List<string>();
            foreach (var sp in patterns)
            {
                files.AddRange(Directory.GetFiles(path, sp, option));
            }

            files.Sort();
            return files;
        }
    }
}