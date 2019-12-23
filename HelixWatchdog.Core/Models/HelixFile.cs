using System.Collections.Generic;

namespace HelixWatchdog.Core.Models
{
    public class HelixFile
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public Dictionary<string, int> References { get; set; }
        public Dictionary<string, int> InvalidReferences { get; set; }

        public HelixFile()
        {
            References = new Dictionary<string, int>();
            InvalidReferences = new Dictionary<string, int>();
        }
    }
}
