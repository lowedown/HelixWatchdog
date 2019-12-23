using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HelixWatchdog.Core.Models;

namespace HelixWatchdog.Core.Services
{
    public class HelixFactory
    {
        public HelixModule GetModule(string relativePath)
        {
            var parts = relativePath.Split('\\');
            var layer = parts[0];
            var moduleName = (parts.Length > 1) ? parts[1] : null;

            Enum.TryParse(layer, true, out HelixLayer helixLayer);

            return new HelixModule() {Layer = helixLayer, Name = moduleName};
        }

        public HelixReference GetReference(string reference)
        {
            var parts = reference.Split('.');

            Enum.TryParse(parts[0], true, out HelixLayer helixLayer);

            return new HelixReference() { 
                Layer = helixLayer, 
                ModuleName = (parts.Length > 1) ? parts[1] : null };
        }

        public IList<HelixReference> ExtractReferences(string content, string namespacePrefix)
        {
            var matches = Regex.Matches(
                content,
                $@"{namespacePrefix}\.(Project|Feature|Foundation|Website)\.\w+\.",
                RegexOptions.IgnoreCase);

            return matches.Select(m => GetReference(m.ToString().Remove(0, namespacePrefix.Length + 1))).ToList();
        }
    }
}
