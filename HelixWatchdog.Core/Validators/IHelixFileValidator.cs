using HelixWatchdog.Core.Models;

namespace HelixWatchdog.Core.Validators
{
    public interface IHelixFileValidator
    {
        bool IsValid(HelixReference reference, HelixModule module, HelixFile file);
    }
}
