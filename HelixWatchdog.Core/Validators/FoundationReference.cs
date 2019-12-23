using HelixWatchdog.Core.Models;

namespace HelixWatchdog.Core.Validators
{
    public class FoundationReference : IHelixFileValidator
    {
        public bool IsValid(HelixReference reference, HelixModule module, HelixFile file)
        {
            return reference.Layer == HelixLayer.Foundation;
        }
    }
}
