using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HelixWatchdog.Core.Services
{
    public class FileSystemService
    {
        public IList<string> GetFiles(string path, string pattern)
        {
            return getFiles(path, pattern, System.IO.SearchOption.AllDirectories)
                .Where(f => !f.Contains("\\obj\\"))
                .Where(f => !f.Contains("\\bin\\")).ToList();
        }

        public string GetFileContent(string file)
        {
            return File.ReadAllText(file);
        }

        private static string[] getFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] searchPatterns = searchPattern.Split('|');
            List<string> files = new List<string>();
            foreach (string sp in searchPatterns)
                files.AddRange(Directory.GetFiles(path, sp, searchOption));
            files.Sort();
            return files.ToArray();
        }
    }
}
