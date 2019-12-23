using System;
using System.Collections.Generic;
using System.Linq;
using HelixWatchdog.Core.Models;
using HelixWatchdog.Core.Validators;

namespace HelixWatchdog.Core.Services
{
    public class FileScannerService
    {
        private readonly FileSystemService _fileSystem;
        private readonly HelixFactory _factory;
        private readonly IList<IHelixFileValidator> _validators;

        public FileScannerService(FileSystemService fileSystem, HelixFactory factory, IList<IHelixFileValidator> validators)
        {
            _fileSystem = fileSystem;
            _factory = factory;
            _validators = validators;
        }

        public IList<HelixModule> ScanFiles(string startPath, string pattern, string namespacePrefix)
        {
            if (!startPath.EndsWith("\\"))
            {
                startPath += "\\";
            }

            IList<string> files = _fileSystem.GetFiles(startPath, pattern);

            var modules = new Dictionary<string, HelixModule>();

            foreach (var dir in files)
            {
                var relativePath = dir.Remove(0, startPath.Length);

                var module = _factory.GetModule(relativePath);

                var moduleKey = string.Concat(module.Layer, ".", module.Name);
                if (modules.ContainsKey(moduleKey))
                {
                    module = modules[moduleKey];
                }
                else
                {
                    modules.Add(moduleKey, module);
                }

                var helixFile = new HelixFile() { Path = dir, Name = relativePath };
                module.Files.Add(helixFile);

                foreach (var reference in _factory.ExtractReferences(_fileSystem.GetFileContent(dir), namespacePrefix))
                {
                    // Skip self-references
                    if (reference.Layer == module.Layer &&
                        reference.ModuleName.Equals(module.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var referenceKey = string.Concat(reference.Layer, ".", reference.ModuleName);

                    if (helixFile.References.ContainsKey(referenceKey))
                    {
                        helixFile.References[referenceKey] = helixFile.References[referenceKey] + 1;
                    }
                    else
                    {
                        helixFile.References.Add(referenceKey, 1);
                    }

                    // Validate
                    var isValid = false;
                    foreach (var validator in _validators)
                    {
                        if (validator.IsValid(reference, module, helixFile))
                        {
                            isValid = true;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        continue;
                    }

                    if (helixFile.InvalidReferences.ContainsKey(referenceKey))
                    {
                        helixFile.InvalidReferences[referenceKey] = helixFile.InvalidReferences[referenceKey] + 1;
                    }
                    else
                    {
                        helixFile.InvalidReferences.Add(referenceKey, 1);
                    }
                }
            }

            return modules.Values.ToList();
        }
    }
}
