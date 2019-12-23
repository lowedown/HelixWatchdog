using HelixWatchdog.Core.Models;

namespace HelixWatchdog.Core.Validators
{
    public class ProjectToFeatureReference : IHelixFileValidator
    {
        public bool IsValid(HelixReference reference, HelixModule module, HelixFile file)
        {
            return module.Layer == HelixLayer.Project && reference.Layer == HelixLayer.Feature;
        }
    }
}
