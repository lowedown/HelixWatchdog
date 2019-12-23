using HelixWatchdog.Core.Models;

namespace HelixWatchdog.Core.Validators
{
    public class WebsiteToAnyReference : IHelixFileValidator
    {
        public bool IsValid(HelixReference reference, HelixModule module, HelixFile file)
        {
            return module.Layer == HelixLayer.Website;
        }
    }
}
