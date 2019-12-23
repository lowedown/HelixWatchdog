using System.Collections.Generic;

namespace HelixWatchdog.Core.Models
{
    public class HelixModule
    {
        public string Name { get; set; }
        public HelixLayer Layer { get; set; }
        public IList<HelixFile> Files { get; set; }

        public HelixModule()
        {
            Files = new List<HelixFile>();
        }
    }
}
